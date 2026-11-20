using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace JarvisFire.Desktop;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Incident> _incidents = new();
    private readonly string _storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JarvisFire", "incidents-demo.json");

    public MainWindow()
    {
        InitializeComponent();
        IncidentList.ItemsSource = _incidents;
        LoadIncidents();
    }

    private void Section_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string section }) return;
        PlaceholderPanel.Visibility = section == "Вызовы" ? Visibility.Collapsed : Visibility.Visible;
        IncidentsPanel.Visibility = section == "Вызовы" ? Visibility.Visible : Visibility.Collapsed;
        SectionTitle.Text = section;
    }

    private void AddIncident_Click(object sender, RoutedEventArgs e)
    {
        var address = AddressBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(address))
        {
            MessageBox.Show("Укажите адрес.", "Проверка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        _incidents.Insert(0, new Incident { Id = Guid.NewGuid(), CreatedAt = DateTimeOffset.Now, Address = address, Description = DescriptionBox.Text.Trim(), Status = "Новый" });
        SaveIncidents();
        AddressBox.Clear();
        DescriptionBox.Clear();
    }

    private void CloseIncident_Click(object sender, RoutedEventArgs e)
    {
        if (IncidentList.SelectedItem is not Incident incident) return;
        incident.Status = "Закрыт";
        IncidentList.Items.Refresh();
        SaveIncidents();
    }

    private void LoadIncidents()
    {
        try
        {
            if (!File.Exists(_storagePath)) return;
            var items = JsonSerializer.Deserialize<Incident[]>(File.ReadAllText(_storagePath));
            foreach (var item in items ?? Array.Empty<Incident>()) _incidents.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось прочитать локальный журнал: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveIncidents()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_storagePath)!);
            var temp = _storagePath + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(_incidents.ToArray(), new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temp, _storagePath, true);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось сохранить локальный журнал: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

public sealed class Incident
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Address { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Новый";
    public string DisplayText => $"{CreatedAt:dd.MM.yyyy HH:mm}  |  {Status}  |  {Address}  |  {Description}";
}