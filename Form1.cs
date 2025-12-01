using AgentForms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgentForms
{
    public partial class Form1 : Form
    {
        private readonly object _lock = new object();
        private List<Process> _currentProcesses = new List<Process>();

        // Sorting state
        private int _sortColumn = 0;
        private SortOrder _sortOrder = SortOrder.Descending;
        private ProcPidi? CurrentPidi = null;

        public Form1()
        {
            InitializeComponent();
            listViewProcesses.SmallImageList = new ImageList();
            timerUpdate.Start();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            _ = RefreshProcessesAsync();
        }

        private async Task RefreshProcessesAsync()
        {
            var filter = string.Empty;
            try
            {
                // Read filter on UI thread
                if (txtFilter.InvokeRequired)
                    txtFilter.Invoke((Action)(() => filter = txtFilter.Text));
                else
                    filter = txtFilter.Text;
            }
            catch { filter = string.Empty; }

            List<Process> procs;
            try
            {
                procs = Process.GetProcesses().ToList();
            }
            catch (Exception ex)
            {
                UpdateStatus("Error listing processes: " + ex.Message);
                return;
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var f = filter.Trim().ToLowerInvariant();
                procs = procs.Where(p =>
                {
                    try
                    {
                        return (p.ProcessName ?? string.Empty).ToLowerInvariant().Contains(f);
                    }
                    catch { return false; }
                }).ToList();
            }

            // Initial sort by memory desc
            procs = SortProcesses(procs, _sortColumn, _sortOrder);

            lock (_lock)
            {
                _currentProcesses = procs;
            }

            await PopulateListViewAsync(procs).ConfigureAwait(false);
            UpdateStatus($"{procs.Count} processes listed. Last update: {DateTime.Now:T}");
        }

        private List<Process> SortProcesses(List<Process> procs, int column, SortOrder order)
        {
            Func<Process, object> keySelector = column switch
            {
                0 => new Func<Process, object>(p => GetWorkingSetSafely(p)), // Memory
                1 => new Func<Process, object>(p => p.ProcessName ?? string.Empty),
                2 => new Func<Process, object>(p => p.Id),
                3 => new Func<Process, object>(p => {
                    try { return p.Threads.Count; } catch { return 0; }
                }),
                4 => new Func<Process, object>(p => {
                    try { return p.StartTime; } catch { return DateTime.MinValue; }
                }),
                5 => new Func<Process, object>(p => {
                    try { return p.Responding; } catch { return false; }
                }),
                _ => new Func<Process, object>(p => GetWorkingSetSafely(p)),
            };

            return (order == SortOrder.Descending)
                ? procs.OrderByDescending(keySelector).ToList()
                : procs.OrderBy(keySelector).ToList();
        }

        private static long GetWorkingSetSafely(Process p)
        {
            try { return p.WorkingSet64; }
            catch { return 0; }
        }

        private Task PopulateListViewAsync(List<Process> procs)
        {
            if (listViewProcesses.InvokeRequired)
            {
                return (Task)listViewProcesses.Invoke((Func<Task>)(() => PopulateListViewAsync(procs)));
            }

            listViewProcesses.BeginUpdate();
            try
            {
                listViewProcesses.Items.Clear();
                foreach (var p in procs)
                {
                    string name = p.ProcessName;
                    string pid = p.Id.ToString();
                    string threads = "-";
                    string memory = "-";
                    string startTime = "-";
                    string responding = "-";

                    try { threads = p.Threads.Count.ToString(); } catch { }
                    try { memory = (p.WorkingSet64 / (1024 * 1024)).ToString("N0"); } catch { }
                    try { startTime = p.StartTime.ToString(); } catch { }
                    try { responding = p.Responding ? "Yes" : "No"; } catch { }

                    // Columns now: Memory, Process, PID, Threads, StartTime, Responding
                    var item = new ListViewItem(new[] { memory, name, pid, threads, startTime, responding }) { Tag = p.Id };
                    listViewProcesses.Items.Add(item);
                }

                // Update header sort glyph (best-effort)
                UpdateColumnHeaderSortGlyph();
            }
            finally
            {
                listViewProcesses.EndUpdate();
            }

            return Task.CompletedTask;
        }

        private void UpdateStatus(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke((Action)(() => lblStatus.Text = text));
            }
            else
            {
                lblStatus.Text = text;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _ = RefreshProcessesAsync();
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {
            if (listViewProcesses.SelectedItems.Count == 0) return;
            var item = listViewProcesses.SelectedItems[0];
            if (item.Tag is int pid)
            {
                try
                {
                    var proc = Process.GetProcessById(pid);
                    var result = MessageBox.Show(this, $"Kill process {proc.ProcessName} (PID {pid})?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        proc.Kill(true);
                        UpdateStatus($"Killed {proc.ProcessName} ({pid})");
                        _ = RefreshProcessesAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Unable to kill process: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listViewProcesses_DoubleClick(object sender, EventArgs e)
        {
            if (listViewProcesses.SelectedItems.Count == 0) return;
            var item = listViewProcesses.SelectedItems[0];
            if (item.Tag is int pid)
            {
                try
                {
                    var proc = Process.GetProcessById(pid);
                    string? path = null;
                    try { path = proc.MainModule?.FileName; } catch { }
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        var dir = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(dir)) Process.Start(new ProcessStartInfo("explorer", dir) { UseShellExecute = true });
                    }
                    else
                    {
                        MessageBox.Show(this, $"Cannot determine executable path for process {proc.ProcessName}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error opening process location: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                _ = RefreshProcessesAsync();
            }
        }

        private void listViewProcesses_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumn)
            {
                // Toggle order
                _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _sortColumn = e.Column;
                _sortOrder = SortOrder.Descending;
            }

            // Re-sort current list and refresh
            lock (_lock)
            {
                _currentProcesses = SortProcesses(_currentProcesses, _sortColumn, _sortOrder);
            }

            _ = PopulateListViewAsync(_currentProcesses);
        }

        private void UpdateColumnHeaderSortGlyph()
        {
            // Windows Forms ListView doesn't support header glyphs without custom drawing.
            // We'll update the column text to include an arrow indicator as a simple visual cue.
            for (int i = 0; i < listViewProcesses.Columns.Count; i++)
            {
                var col = listViewProcesses.Columns[i];
                var text = col.Text;
                // Strip existing arrows
                text = text.Replace(" ?", "").Replace(" ?", "");
                if (i == _sortColumn)
                {
                    text += _sortOrder == SortOrder.Descending ? " ?" : " ?";
                }
                col.Text = text;
            }
        }

        private void btnRunElevated_Click(object sender, EventArgs e)
        {
            // If a process is selected, try to start that process elevated.
            if (listViewProcesses.SelectedItems.Count > 0)
            {
                var item = listViewProcesses.SelectedItems[0];
                if (item.Tag is int pid)
                {
                    try
                    {
                        var proc = Process.GetProcessById(pid);
                        string? path = null;
                        try { path = proc.MainModule?.FileName; } catch { }
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            StartProcessElevated(path);
                        }
                        else
                        {
                            MessageBox.Show(this, "Cannot determine executable path to run elevated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Unable to start elevated: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return;
                }
            }

            // Otherwise restart this application elevated
            var result = MessageBox.Show(this, "Restart this application with elevated privileges?", "Run Elevated", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    RestartElevated();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Failed to restart elevated: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RestartElevated()
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath)) throw new InvalidOperationException("Cannot determine executable path.");

            var psi = new ProcessStartInfo(exePath)
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(psi);
            Application.Exit();
        }

        private void StartProcessElevated(string path)
        {
            var psi = new ProcessStartInfo(path)
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(psi);
            }
            catch (Win32Exception wx)
            {
                MessageBox.Show(this, "User declined elevation or failed: " + wx.Message, "Elevation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void listViewProcesses_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listViewProcesses.SelectedItems.Count == 0)
            {
                CurrentPidi = null;
                return;
            }

            var item = listViewProcesses.SelectedItems[0];
            if (item.Tag is int pid)
            {
                try
                {
                    var pidi = new ProcPidi(pid);
                    try
                    {
                        var proc = pidi.Proc;
                        if (proc != null)
                        {
                            try { pidi.Path = proc.MainModule?.FileName ?? string.Empty; } catch { pidi.Path = string.Empty; }
                        }
                    }
                    catch { }

                    CurrentPidi = pidi;
                    var procId = CurrentPidi?.Proc?.Id ?? -1;
                    //var vm = CurrentPidi?.Proc?.WorkingSet64 ?? 0;
                    //int vmKB = (int)vm / 1024;
                    //int vmMB = (int)vm / (1024 * 1024);
                    //double vmGB = (double)vm / (1024 * 1024 * 1024);
                   
                    UpdateStatus($"Selected PID: {pid}, CurrentPidi Proc's Id is {procId}, VM: {CurrentPidi?.WorkingMemoryMB} MB, {CurrentPidi?.WorkingMemoryGB} GB.");



                }
                catch (Exception ex)
                {
                    // If process cannot be accessed, clear selection wrapper
                    CurrentPidi = null;
                    UpdateStatus("Selected process unavailable: " + ex.Message);
                }
            }
            else
            {
                CurrentPidi = null;
            }
        }
    }
}
