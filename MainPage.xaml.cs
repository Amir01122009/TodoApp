using System.Collections.ObjectModel;
using System.Formats.Tar;
using System.Text.Json;

namespace TodoApp;

public partial class MainPage : ContentPage
{
    public ObservableCollection<TaskItem> Tasks { get; set; } = new ObservableCollection<TaskItem>();
    private readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.json");

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadTasksAsync();
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(TaskEntry.Text))
        {
            Tasks.Add(new TaskItem { Title = TaskEntry.Text, IsCompleted = false });
            TaskEntry.Text = string.Empty;
        }
        else
        {
            await DisplayAlert("Ошибка", "Введите название задачи!", "OK");
        }
    }

    private async void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem task)
        {
            task.IsCompleted = e.Value;
            await SaveTasksAsync();
        }
    }

    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is TaskItem task)
        {
            Tasks.Remove(task);
            await SaveTasksAsync();
        }
    }

    private async void OnSaveTasksClicked(object sender, EventArgs e)
    {
        await SaveTasksAsync();
        await DisplayAlert("Успех", "Задачи сохранены!", "OK");
    }

    private async Task SaveTasksAsync()
    {
        try
        {
            string json = JsonSerializer.Serialize(Tasks);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
        }
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                var loadedTasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                foreach (var task in loadedTasks)
                {
                    Tasks.Add(task);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить: {ex.Message}", "OK");
        }
    }
}

public class TaskItem
{
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}