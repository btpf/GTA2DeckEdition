using GTA2ModManager48.Properties;
using SharpDX.XInput;
using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace GTA2ModManager48
{

    public partial class Form1 : Form
    {
        static String currentDirectory;
        Controller controller = new Controller(UserIndex.One);
        Timer timer = new Timer();
        Dictionary<string, string> folderMap = new Dictionary<string, string>();


        String currentlyLoadedMod = "";
        String currentlyLoadedModPersistPath = "";
        Boolean DarkTheme = false;
        bool settingMenu = false;

        // Allow for a smooth button pressing experience
        int intervalsPassed = 0;
        State previousState;

        // Prevent nothing from being selected in the listview
        int previousListViewIndice = 0;

        private enum Direction
        {
            UP,
            DOWN,
            MIDDLE
        }

        Direction previousDirection = Direction.MIDDLE;


        public Form1()
        {
            InitializeComponent();

            if (controller.IsConnected)
            {
                timer.Interval = 50;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            currentDirectory = Directory.GetCurrentDirectory();



            {
                Image original = AButton.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                AButton.Image = resized;
            }
            {
                Image original = BButton.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                BButton.Image = resized;
            }
            {
                Image original = XButton.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                XButton.Image = resized;
            }
            {
                Image original = YButton.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                YButton.Image = resized;
            }
            {
                Image original = StartButton.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                StartButton.Image = resized;
            }
            {
                Image original = button8.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                button8.Image = resized;
            }
            {
                Image original = button9.Image;
                Image resized = new Bitmap(original, new Size(30, 30));
                button9.Image = resized;
            }
        }

        private void updateModLoadedText(String modName)
        {
            if (modName == "")
            {
                modName = "None";
            }
            label1.Text = "Mod Loaded: " + modName;

        }
        private void Form1_Load(object sender, EventArgs _event)
        {


            // Lower level theming stuff
            // ThemeAllControls();



            if (true)
            {

            listView1.DrawColumnHeader += (s, e) => e.DrawDefault = true;

            
              listView1.DrawItem += (s, e) =>
            {
                // Only draw the background here, not text — text comes in DrawSubItem
                if (e.Item.Selected)
                    e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                else
                    e.DrawBackground();
            };
            

            listView1.DrawSubItem += (s, e) =>
            {


                bool selected = e.Item.Selected;
                if (e.ColumnIndex == 0)
                {
                    // Let the system draw the first column (with image)
                    if (selected)
                    {
                        e.Graphics.FillRectangle(Brushes.White, e.Bounds);

                    }

                    else {
                        e.DrawBackground();
                    }


                    if (e.Item.ImageKey != "")
                    {
                        Image img = listView1.SmallImageList.Images[0];
                        int imgX = e.Bounds.Left + 2;
                        int imgY = e.Bounds.Top + (e.Bounds.Height - img.Height) / 2;
                        e.Graphics.DrawImage(img, imgX, imgY);
                    }

                    return;

                }
                // Draw background
                if (selected)
                    e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                else
                    e.DrawBackground();

                // Draw text at proper position
                TextRenderer.DrawText(
                    e.Graphics,
                    e.SubItem.Text,
                    e.Item.Font,
                    e.Bounds,
                    selected ? Color.Black : listView1.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );
            };

            }


            fullScreenPreview.Visible = false;
            fullScreenPreview.BringToFront();
            if (!Directory.Exists("../MapMods"))
            {
                Directory.CreateDirectory("../MapMods");
            }
            string[] directories = Directory.GetDirectories(currentDirectory + "/../MapMods");

            {
                ListViewItem t = new ListViewItem();
                t.SubItems.Add($"No Mods/Unload Mods");
                t.Font = new Font("Segoe UI", 12);
                listView1.Items.Add(t);
            }


            foreach (String directory in directories)
            {
                string folderName = Path.GetFileName(directory);
                //listBox1.Items.Add(folderName);

                ListViewItem t = new ListViewItem();

                if (File.Exists(directory + "\\.starred"))
                {
                    t.ImageKey = "heart";
                }
                t.SubItems.Add($"{folderName}");
                t.Font = new Font("Segoe UI", 12);
                //t.SubItems.Add("--tyty");

                listView1.Items.Add(t);

                folderMap[folderName] = directory;
            }

            listView1.ListViewItemSorter = new CustomSorter();
            listView1.Sort();

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            string darkModePath = Path.Combine(exeDir, ".darkmode");

            if (File.Exists(darkModePath))
            {
                setDarkMode();
            }


            string filePath = Path.Combine(exeDir, "ModLoaded.txt");

            currentlyLoadedModPersistPath = filePath;

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
            }
            else
            {
                currentlyLoadedMod = File.ReadAllText(filePath);
                updateModLoadedText(currentlyLoadedMod);
            }

            //listView1.SelectedItems.Clear();
            listView1.Items[0].Selected = true;
            listView1.EnsureVisible(0);

            if (!String.IsNullOrEmpty(currentlyLoadedMod))
            {
                //listBox1.SelectedIndex = listBox1.Items.IndexOf(currentlyLoadedMod);
                selectCurrentlyLoadedModInListView();

            }
        }

        private void selectCurrentlyLoadedModInListView()
        {
            //listView1.SelectedItems.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[1].Text == currentlyLoadedMod)
                {
                    selectIndexInListView(item.Index);
                    break;
                }
            }
        }

        private void selectIndexInListView(int index)
        {
            //listView1.SelectedItems.Clear();
            listView1.Items[index].Selected = true;
            listView1.Items[index].Focused = true;
            previousListViewIndice = index;
            listView1.EnsureVisible(index);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.ContainsFocus)
                return;


            State currentState = controller.GetState();
            var currButtons = currentState.Gamepad.Buttons;
            var prevButtons = previousState.Gamepad.Buttons;

            intervalsPassed += 1;
            var currentDirection = Direction.MIDDLE;

            if ((currButtons == GamepadButtonFlags.DPadDown) || currentState.Gamepad.LeftThumbY < -32767 * 0.75)
            {
                currentDirection = Direction.UP;

            }
            else if ((currButtons == GamepadButtonFlags.DPadUp) || currentState.Gamepad.LeftThumbY > 32767 * 0.75)
            {
                currentDirection = Direction.DOWN;
            } else if ((currButtons == GamepadButtonFlags.DPadLeft) || currentState.Gamepad.RightThumbY > 32767 * 0.75)
            {
                ScrollRichTextBox(richTextBox1, -1);
                }
            else if ((currButtons == GamepadButtonFlags.DPadRight) || currentState.Gamepad.RightThumbY < -32767 * 0.75)
            {
                ScrollRichTextBox(richTextBox1, 1);

            }

            if (intervalsPassed == 1 || intervalsPassed > 8)
            {
                if ((currButtons & GamepadButtonFlags.A) != 0)
                {
                    if (settingMenu)
                    {
                        Control focused = this.ActiveControl;

                        if (focused is Button btn)
                        {
                            btn.PerformClick(); // Simulate a button press
                        }
                        return;
                    }
                    AButton.PerformClick();
                }
                else if ((currButtons & GamepadButtonFlags.X) != 0)
                {
                    if (settingMenu)
                    {

                        return;
                    }
                    XButton.PerformClick();
                }
                else if ((currButtons & GamepadButtonFlags.Y) != 0)
                {
                    if (settingMenu)
                    {

                        return;
                    }
                    YButton.PerformClick();

                }
                else if ((currButtons & GamepadButtonFlags.B) != 0)
                {
                    if (settingMenu)
                    {
                        panel1.SendToBack();
                        panel1.Visible = false;
                        settingMenu = false;
                        return;
                    }
                    BButton.PerformClick();

                }
                else if ((currButtons & GamepadButtonFlags.Start) != 0)
                {
                    if (settingMenu)
                    {

                        return;
                    }
                    StartButton.PerformClick();
                }
                else if ((currButtons & GamepadButtonFlags.RightShoulder) != 0 || (currButtons & GamepadButtonFlags.LeftShoulder) != 0)
                {
                    settingMenu = !settingMenu;
                    if (settingMenu)
                    {
                        panel1.BringToFront();
                        panel1.Visible = true;

                    }
                    else
                    { 
                        panel1.Visible = false;
                    }
                }
                else if (currentDirection == Direction.UP)
                {
                    if (settingMenu)
                    {
                        this.SelectNextControl(this.ActiveControl, true, true, true, true);
                        return;
                    }
                    //listBox1.SelectedIndex = Math.Min(listBox1.SelectedIndex + 1, listBox1.Items.Count - 1);
                    selectIndexInListView(Math.Min(listView1.SelectedIndices[0] + 1, listView1.Items.Count - 1));

                }
                else if (currentDirection == Direction.DOWN)
                {
                    if (settingMenu)
                    {
                        this.SelectNextControl(this.ActiveControl, false, true, true, true);
                        return;
                    }
                    //listBox1.SelectedIndex = Math.Max(listBox1.SelectedIndex - 1, 0);

                    selectIndexInListView(Math.Max(listView1.SelectedIndices[0] - 1, 0));

                }

                else
                {
                    intervalsPassed = 0;
                }
            }
            else
            {
                if (currButtons != prevButtons || currentDirection != previousDirection)
                {
                    intervalsPassed = 0;
                }
            }
            previousState = currentState;
            previousDirection = currentDirection;

        }



        /*
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= 0)
            {
                string text = listBox1.Items[e.Index].ToString();
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;   // Horizontal center
                    sf.LineAlignment = StringAlignment.Center; // Vertical center

                    using (Brush brush = new SolidBrush(e.ForeColor))
                    {
                        e.Graphics.DrawString(
                            text,
                            e.Font,
                            brush,
                            e.Bounds,
                            sf
                        );
                    }
                }
            }

            e.DrawFocusRectangle();
        }
        */

        private void XButton_Click(object sender, EventArgs e)
        {
            fullScreenPreview.Visible = !fullScreenPreview.Visible;
        }

        private void AButton_Click(object sender, EventArgs e)
        {

            //String currentlySelectedMod = listBox1.SelectedItem?.ToString();

            string currentlySelectedMod = listView1.SelectedItems[0].SubItems[1].Text;

            if (!String.IsNullOrEmpty(currentlyLoadedMod))
            {
                Debug.WriteLine("Before reverse", currentlyLoadedMod);
                if (folderMap.ContainsKey(currentlyLoadedMod))
                {
                    Debug.WriteLine("REVERSING");
                    Helper.ReverseOriginalRestore(folderMap[currentlyLoadedMod] + "\\data", currentDirectory + "\\..\\data");
                }
            }

            if (currentlySelectedMod == "No Mods/Unload Mods")
            {
                currentlyLoadedMod = "";
                updateModLoadedText(currentlyLoadedMod);
                File.WriteAllText(currentlyLoadedModPersistPath, currentlyLoadedMod);
            }

            if (folderMap.ContainsKey(currentlySelectedMod))
            {
                String modDirectory = folderMap[currentlySelectedMod];
                Helper.CopyWithStructureAndBackup(modDirectory + "\\data", currentDirectory + "..\\..\\data");

                currentlyLoadedMod = currentlySelectedMod;
                updateModLoadedText(currentlyLoadedMod);
                File.WriteAllText(currentlyLoadedModPersistPath, currentlyLoadedMod);

            }
        }

        private void BButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void YButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices[0] == 0) { return; }
            string selectedItem = listView1.SelectedItems[0].SubItems[1].Text;

            string[] directories = Directory.GetDirectories(currentDirectory + "/../MapMods");
            string starredPath = currentDirectory + "\\..\\MapMods\\" + selectedItem + "\\.starred";

            if (File.Exists(starredPath))
            {
                try
                {
                    File.Delete(starredPath);
                    listView1.SelectedItems[0].ImageKey = "";
                }
                catch (Exception)
                {

                }


                //listView1.RedrawItems(listView1.SelectedItems[0].Index, listView1.SelectedItems[0].Index, true);
                //File.Delete
            }
            else
            {
                File.Create(starredPath).Dispose();
                listView1.SelectedItems[0].ImageKey = "heart";
            }

            listView1.Sort();

        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //listView1.Update();        // Forces immediate repaint (optional)



            // Wait a lifecycle
            this.BeginInvoke((MethodInvoker)(() =>
            {
                // This is to fix wine linux artifacts
                if(DarkTheme) listView1.Invalidate();    // Marks control for repaint

                // Checks if the listview has actually been deselected
                if (listView1.SelectedItems.Count == 0 && listView1.Items.Count > 0)
                {
                    listView1.Items[previousListViewIndice].Selected = true;
                }
            }));

            if (listView1.SelectedIndices.Count == 0 && listView1.Items.Count > 0)
            {
                return;
            };

            int selectedIndice = listView1.SelectedIndices[0];
            previousListViewIndice = listView1.SelectedIndices[0];

            string selectedItem = listView1.SelectedItems[0].SubItems[1].Text;
            Debug.WriteLine(selectedItem);


            label2.Text = selectedItem;
            bool thumbnailSet = false;
            if (selectedIndice != 0 && folderMap.ContainsKey(selectedItem))
            {
                String modDirectory = folderMap[selectedItem];
                String thumbnail = Helper.FindSingleThumbImage(modDirectory);
                
                if (thumbnail != null)
                {
                    thumbnailSet = true;


                    Image original = Image.FromFile(thumbnail);
                    Bitmap clone1 = new Bitmap(original);
                    Bitmap clone2 = new Bitmap(original);

                    // Resizing the image is computationally intensive, especially through wine/proton
                    // So we run this on a seperate thread to make the experience smoother
                    
                    
                    Task.Run(() =>
                    {
                        //Image clone = new Bitmap(original);

                        // Resize to simulate Zoom
                        var resized = Helper.ResizeAndCenter_LockBits(clone1, pictureBox1.ClientSize);
                        

                        pictureBox1.Invoke((MethodInvoker)(() =>
                        {
                            pictureBox1.Image = resized;
                        }));
                        


                    });
                    
                    

                    Task.Run(() =>
                    {
                        //Image clone = new Bitmap(original);
                        // Resize to simulate Zoom
                        var resized = Helper.ResizeAndCenter_LockBits(clone2, fullScreenPreview.ClientSize);

                        fullScreenPreview.Invoke((MethodInvoker)(() =>
                        {
                            fullScreenPreview.Image = resized;
                        }));


                    });
                    //original.Dispose();
                }
                String description = File.ReadAllText(modDirectory + "\\readme.txt");
                richTextBox1.Text = description;
            }

            if(selectedIndice == 0 || !thumbnailSet)
            {
                Task.Run(() =>
                {
                    Bitmap original = Properties.Resources.GTA2_Box_art;
                    var resized = Helper.ResizeAndCenter_LockBits(original, pictureBox1.ClientSize);

                    pictureBox1.Invoke((MethodInvoker)(() =>
                    {
                        pictureBox1.Image = resized;
                    }));

                    resized = Helper.ResizeAndCenter_LockBits(original, fullScreenPreview.ClientSize);
                    fullScreenPreview.Invoke((MethodInvoker)(() =>
                    {
                        fullScreenPreview.Image = resized;
                    }));


                });
                if (selectedIndice == 0)
                {
                    label2.Text = "Grand Theft Auto 2";
                    richTextBox1.Text = "Activate This Entry To Unload All Map Mods";
                    return;
                }
            }
        }

        private void fullScreenPreview_Click(object sender, EventArgs e)
        {
            fullScreenPreview.Visible = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string gta2Path = currentDirectory + "..\\..\\gta2.exe";

            if (File.Exists(gta2Path))
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = gta2Path,
                    WorkingDirectory = Path.GetDirectoryName(gta2Path)
                };

                Process.Start(psi);
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
        }

        private void fullScreenPreview_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                fullScreenPreview.Visible = false;
            }
        }

        private void fullScreenPreview_VisibleChanged(object sender, EventArgs e)
        {
            fullScreenPreview.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            fullScreenPreview.Visible = true;
            fullScreenPreview.BringToFront();
        }



        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SB_BOTH = 3;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);




        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);


        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        // https://stackoverflow.com/questions/72988434/how-to-make-winform-use-the-system-dark-mode-theme
        // https://github.com/edgars-pivovarenoks/winforms-modernui/blob/master/MetroFramework/Native/DwmApi.cs

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20; // Windows 10 1809 and newer

        private void ThemeAllControls(int darkMode = 1, Control parent = null)
        {
            parent = parent ?? this;
            Action<Control> Theme = control => {
                int trueValue = darkMode;
                SetWindowTheme(control.Handle, darkMode == 1?"DarkMode_Explorer":"Explorer", null);
                DwmSetWindowAttribute(control.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));
            };
            if (parent == this) Theme(this);
            foreach (Control control in parent.Controls)
            {
                Theme(control);
                if (control.Controls.Count != 0)
                    ThemeAllControls(darkMode, control);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < richTextBox1.TextLength; i++)
            {
                richTextBox1.Select(i, 1);
                richTextBox1.SelectionColor = Color.Red;
            }
            richTextBox1.Select(0, 0); // Clear selection
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionColor = Color.White;
            richTextBox1.AppendText("This is white text\n");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!DarkTheme) return;
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.White;
            richTextBox1.DeselectAll();
        }









        /*


        // Fields to track drag state
        private bool isDragging = false;
        private Point lastMousePos;

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePos = e.Location;
                richTextBox1.Cursor = Cursors.SizeAll;  // Change cursor to grabbing hand
                label2.Focus();
            }
        }

        private void richTextBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int dy = lastMousePos.Y - e.Y;

                // Estimate line height based on font size
                int lineHeight = TextRenderer.MeasureText("A", richTextBox1.Font).Height;

                // Convert pixel delta to lines
                int linesToScroll = dy / lineHeight;

                if (linesToScroll != 0)
                {
                    ScrollRichTextBox(richTextBox1, linesToScroll);
                    lastMousePos = e.Location;
                }
            }
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                richTextBox1.Cursor = Cursors.IBeam;
            }
        }

         */

        const int EM_LINESCROLL = 0x00B6;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private void ScrollRichTextBox(RichTextBox rtb, int dy)
        {
            // wParam = horizontal scroll amount (0 here), lParam = vertical scroll amount
            SendMessage(rtb.Handle, EM_LINESCROLL, IntPtr.Zero, (IntPtr)dy);
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionLength = 0;
        }


        private void SetBackColorAllControls(Control parent, Color backgroundColor, Color foregroundColor)
        {
            foreach (Control control in parent.Controls)
            {

                try
                {
                    control.ForeColor = foregroundColor;
                }
                catch (Exception)
                {

                    throw;
                }

                try
                {
                    control.BackColor = backgroundColor;
                }
                catch (Exception)
                {

                }

                try
                {
                    //control.Invalidate();
                    control.Refresh();
                }
                catch (Exception)
                {

                    throw;
                }
                

                // Recursively set for child controls
                if (control.HasChildren)
                {
                    SetBackColorAllControls(control, backgroundColor, foregroundColor);
                }
            }
        }

        void setDarkMode()
        {
            DarkTheme = true;
            ThemeAllControls();
            SetBackColorAllControls(this, Color.FromArgb(23, 29, 37), Color.White);
            listView1.OwnerDraw = true;
            this.BackColor = Color.FromArgb(23, 29, 37);
            label1.ForeColor = Color.Coral;
        }

        void setLightMode()
        {
            DarkTheme = false;
            ThemeAllControls(0);
            SetBackColorAllControls(this, SystemColors.Control, Color.Black);
            richTextBox1.BackColor = SystemColors.Control;
            pictureBox1.BackColor = Color.Transparent;
            listView1.OwnerDraw = false;
            this.BackColor = SystemColors.Control;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowScrollBar(listView1.Handle, SB_HORZ, false);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ShowScrollBar(listView1.Handle, SB_HORZ, false);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists(currentDirectory + "\\..\\scripts\\GTA2Radar.asi"))
            {
                File.Move(currentDirectory + "\\..\\scripts\\GTA2Radar.asi", currentDirectory + "\\..\\scripts\\GTA2Radar.asi.original");
            }
            else if(File.Exists(currentDirectory + "\\..\\scripts\\GTA2Radar.asi.original"))
            {
                File.Move(currentDirectory + "\\..\\scripts\\GTA2Radar.asi.original", currentDirectory + "\\..\\scripts\\GTA2Radar.asi");
            }

            redrawSettingsPanel();

        }

        private void button7_Leave(object sender, EventArgs e)
        {


                if (this.ActiveControl?.TabIndex > button7?.TabIndex || this.ActiveControl?.TabIndex == 0)
                {
                            button7.Focus();

                }

        }

        private void button5_Leave(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)(() =>
            {

                if (this.ActiveControl?.TabIndex < button5.TabIndex)
                {
                    button5.Focus();

                }
            }));



        }

        private void button5_Enter(object sender, EventArgs e)
        {
            Debug.Write("Focus here");


        }

        private void redrawSettingsPanel()
        {
            if (DarkTheme)
            {
                setDarkMode();
            }
            else
            {
                setLightMode();
            }
            //button5.ForeColor = DarkTheme? Color.SpringGreen: Color.Crimson;
            button5.ForeColor = DarkTheme? Color.SpringGreen: Color.Black;
            button5.Text = DarkTheme ? "Dark" : "Light";

            if (File.Exists(currentDirectory + "\\..\\scripts\\GTA2Radar.asi"))
            {
                button4.Text = "Enabled";
                button4.ForeColor = DarkTheme? Color.SpringGreen:Color.Green;
            }
            else
            {
                button4.Text = "Disabled";
                button4.ForeColor = Color.Crimson;
            }


            String frontEndFixIni = currentDirectory + "\\..\\scripts\\FrontendFixII.ini";

            if (File.Exists(frontEndFixIni))
            {
                int currentVal = getSelectedControlScheme();
                const int baseTabIndex = 102;

                foreach (Control ctrl in panel1.Controls)
                {
                    if (ctrl.TabIndex == baseTabIndex + currentVal)
                    {
                        ctrl.ForeColor = DarkTheme ? Color.SpringGreen : Color.Green;
                        break;
                    }
                }

            }
            



        }

        private int getSelectedControlScheme()
        {
            String frontEndFixIni = currentDirectory + "\\..\\scripts\\FrontendFixII.ini";


                IEnumerable<String> readLines= File.ReadLines(frontEndFixIni);
                if (readLines.Contains("DefaultControls = 0"))
                {
                    return 0;
                }
                if (readLines.Contains("DefaultControls = 1"))
                {
                    return 1;
                }
                if (readLines.Contains("DefaultControls = 2"))
                {
                    return 2;
                }
            

            // Error
                return -1;
            
        }

        private void panel1_VisibleChanged(object sender, EventArgs e)
        {
            redrawSettingsPanel();
            button5.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            string darkModePath = Path.Combine(exeDir, ".darkmode");

            if (DarkTheme)
            {
                File.Delete(darkModePath);
            }
            else
            {
                File.Create(darkModePath).Close();
            }

            if (DarkTheme)
            {
                setLightMode();
            }
            else
            {
                setDarkMode();
            }

            redrawSettingsPanel();
        }
        
        void changeDefaultControl(int newControlID)
        {
            int selected = getSelectedControlScheme();
            String frontEndFixIni = currentDirectory + "\\..\\scripts\\FrontendFixII.ini";

            var updatedLines = File.ReadAllLines(frontEndFixIni)
    .Select(line => line == "DefaultControls = " + selected ? "DefaultControls = " + newControlID : line);

            File.WriteAllLines(frontEndFixIni, updatedLines);

            redrawSettingsPanel();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            changeDefaultControl(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            changeDefaultControl(1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            changeDefaultControl(2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
            panel1.Visible = true;
            settingMenu = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            panel1.SendToBack();
            panel1.Visible = false;
            settingMenu = false;
        }

    }
}
