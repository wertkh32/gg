using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Media.Effects;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Threading;
using System.Reflection;
using gg;
using System.Diagnostics;
using System.ComponentModel;

namespace ggUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       #region variables
       public double RTBWidth = 0, RTBHeight = 0, RTBTop = 0, RTBLeft = 0, txtCmdLineTop = 0, txtErrorTop = 0,FrameTop = 0;
       private int TEXTBOX_NO = 5, RTBIndex = 0, txtShift = 0, RTBId = 0, HistoryListIndex = 0, CmdLineCaretIndex = 0, TUTORIAL_PAGE_NO=17;
       private RichTextBox RTBNow;
       private TextBox txtCmdLine, txtError, txtNextArrow, txtPrevArrow;
       private Label lblBackground;
       private List<RichTextBox> RTBArray;
       private List<string> TagList, HistoryList;
       private List<bool> DeletedTagLists;
       private FontFamily Default_Font = new FontFamily("Liberation Sans");
       private FontFamily Default_Arrow_Font = new FontFamily("Anklada™ Original");
       
       private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255,73,73)),
                               yellow = new SolidColorBrush(Color.FromRgb(255, 255, 150)),
                               green = new SolidColorBrush(Color.FromRgb(150, 255, 120)),
                               blue = new SolidColorBrush(Color.FromRgb(120, 150, 255)),
                               black = new SolidColorBrush(Colors.Black),
                               white = new SolidColorBrush(Colors.White);

       private string[] months = { "January", "Febuary", "March","April", "May", "June", "July", "August", "September", "October", "November", "December" };
       Invoker CmdInvoker;
       System.Windows.Forms.NotifyIcon NotificationIcon;

       private const string ADD_PATTERN = "^(a|add)$",
                            LIST_PATTERN = "^(ls|list)$",
                            CHANGE_PATTERN = "^(ch|change)$",
                            REMOVE_PATTERN = "^(rm|remove)$",
                            SYNC_PATTERN = "^(sync)$",
                            PIN_PATTERN = "^(pin)$",
                            ACT_PATTERN = "^(act)$";

       public const string SettingsFile = "gg.ini";
       public string ShortcutLink = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\gg.lnk";

       public Key ShowHotkey = Key.F12, ExitHotkey = Key.End, ScrollHotkey = Key.RightCtrl;
       public options OptionsWindow;
       public bool OptionsShowing = false, WindowShowing=false, RunOnStartup=true, StolenFocus=false, InTutorialMode=true;
      
#endregion
       #region DLLIMPORT
       [DllImport("user32.dll")]
       static extern int CallNextHookEx(IntPtr hhk, int code, int wParam, ref kbInfo lParam);
       [DllImport("user32.dll")]
       static extern IntPtr SetWindowsHookEx(int idHook, deleHooky callback, IntPtr hInstance, uint theardID);
       [DllImport("user32.dll")]
       static extern bool UnhookWindowsHookEx(IntPtr hInstance);
       [DllImport("kernel32.dll")]
       static extern IntPtr LoadLibrary(string lpFileName);
       [DllImport("user32.dll")]
       static extern short GetKeyState(int vKey);
       [DllImport("user32.dll")]
       static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
       [DllImport("user32.dll")]
       static extern bool SetCursorPos(int X, int Y);

       [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetForegroundWindow();
       [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
       static extern int GetWindowThreadProcessId(int handle, out int processId);
       [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
       static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
       [DllImport("kernel32.dll")]
       static extern int GetCurrentThreadId();
       [DllImport("user32.dll")]
       static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);


       private const int MOUSEEVENTF_LEFTDOWN = 0x02;
       private const int MOUSEEVENTF_LEFTUP = 0x04;
       private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
       private const int MOUSEEVENTF_RIGHTUP = 0x10;
       private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

       deleHooky hookDelegate;
       IntPtr hookHandle=IntPtr.Zero;
       
       private const int WH_KEYBOARD_LL = 13;
       private const int WM_KEYDOWN = 0x0100;
       private const int VK_RETURN = 0X0D; //Enter
       private const int VK_SPACE = 0X20; //Space
       private const int VK_SHIFT = 0x10;
       private const int VK_CAPITAL = 0x14;
       private const int VK_CONTROL = 0x11;
       private const int VK_PAUSE = 0x13;
       const int VK_F12 = 0x7B; 

       public struct kbInfo
       {
           public int vkCode;
           public int scanCode;
           public int flags;
           public int time;
           public int dwExtraInfo;
       }


       public delegate int deleHooky(int Code, int wParam, ref kbInfo lParam);


        #endregion

    

       private void InitTagList()
       {
           TagList = new List<String>(CmdInvoker.GetTagList());
           TagList.Add("All");
           TEXTBOX_NO = TagList.Count();
       }
        private void BeginLogic(){
            CmdInvoker = Invoker.GetInstance();
            InitTagList();
        }
        
        private GGResult GetTagContents(String tag){
            if(tag=="All") return CmdInvoker.InvokeCommand("ls");
            return CmdInvoker.InvokeCommand("ls #" + tag);
        }
        
        private TableRow AddTitle(string Text)
        {
            TableCell tc = new TableCell();
            TableRow tr = new TableRow();
           
            Run r = new Run(Text);
            r.Foreground = black;//new SolidColorBrush(Colors.Red);
            r.FontSize = 24;
            r.FontWeight = FontWeights.Bold;

            Paragraph p = new Paragraph();
          
            p.Inlines.Add(r);
            tc.Blocks.Add(p);

            tc.BorderThickness = new Thickness(0, 10, 0, 10);
            tr.Cells.Add(tc);
            return tr;
        }
        
        private TableCell NewItemCell(string Text)
        {
            TableCell tc = new TableCell();
            Run r = new Run(Text);
            r.Foreground = black;
            r.FontSize = 14;
            r.FontWeight = FontWeights.Medium;
            Paragraph p = new Paragraph();

            p.Inlines.Add(r);
            tc.Blocks.Add(p);
            return tc;
        }
        
        private TableRow AddItemRow(string Text)
        {
            TableRow tr = new TableRow();
            
           // tr.Cells.Add(newItemCell("\t"+INDENT));
           
            string[] parts = Text.Split('|');
            TableCell desc = NewItemCell(parts[0]);
            TableCell tag = NewItemCell(parts[1]);
            TableCell date = NewItemCell(parts[2]);

            date.TextAlignment = TextAlignment.Right;
            
            tr.Cells.Add(desc);
            tr.Cells.Add(tag);
            tr.Cells.Add(date);
          
            return tr;
        }
        
        private TableRowGroup AddTextChunkToTable(string chunk)
        {
            TableRowGroup trg = new TableRowGroup();
            String[] lines = chunk.Split('\n');
            for (int j = 0; j < lines.Count(); j++)
                if(lines[j]!="")trg.Rows.Add(AddItemRow(lines[j]));
            return trg;
        }
        
        private void RefreshContents(int index)
        {
            RTBNow.Document.Blocks.Clear();
            Table t = new Table();
            
            t.Columns.Add(new TableColumn());
            t.Columns.Add(new TableColumn());
            t.Columns.Add(new TableColumn());

            TableRowGroup trg = new TableRowGroup();
            trg.Rows.Add(AddTitle(TagList[index]));
            t.RowGroups.Add(trg);
            t.RowGroups.Add(AddTextChunkToTable(GetTagContents(TagList[index]).GetMainResult()));
            t.BorderThickness = new Thickness(RTBWidth/16,0,0,0);
            t.CellSpacing = 10;

            double tablewidth = RTBWidth * (13.2 / 16.0);
            

            t.Columns[0].Width = new GridLength(tablewidth/2);
            t.Columns[1].Width = new GridLength(tablewidth/4);
            t.Columns[2].Width = new GridLength(tablewidth/4);
            
            RTBNow.Document.Blocks.Add(t);
                     
        }
        
        private void InitTextBoxContents()
        {
            for (int i = 0; i < TagList.Count; i++)
            {
                RefreshContents(i);
            }
        }
        
        private int vkKey(Key k)
       {
           return KeyInterop.VirtualKeyFromKey(k);
       }
        
        public bool IsDown(int vKey)
       {
           return (GetKeyState(vKey) & 0x80) == 0x80;
       }

       public bool IsKeyDown(Key vKey)
       {
           return (GetKeyState(vkKey(vKey)) & 0x80) == 0x80;
       }

       private void wiggle()
       {
           Process sexy = new Process();
           sexy.StartInfo.FileName = "http://www.youtube.com/watch?v=wyx6JDQCslE";
           sexy.StartInfo.UseShellExecute = true;
           sexy.Start();
           
           Run r = new Run("WIGGLE");
           r.Foreground = black;//new SolidColorBrush(Colors.Red);
           r.FontSize = 160;
           r.FontWeight = FontWeights.Bold;
           Paragraph p = new Paragraph(r);

           Run r2 = new Run("\n\t\t\t\tSexy And I Know It");
           r2.Foreground = red;//new SolidColorBrush(Colors.Red);
           r2.FontSize = 32;
           r2.FontWeight = FontWeights.Bold;
           p.Inlines.Add(r2);

           p.Margin = new Thickness(RTBWidth * 0.1, RTBHeight * 0.1, 0, 0);
           RTBNow.Document.Blocks.Clear();
           RTBNow.Document.Blocks.Add(p);

           Storyboard sb = new Storyboard();

           DoubleAnimationUsingKeyFrames fi = new DoubleAnimationUsingKeyFrames();
           fi.BeginTime = TimeSpan.FromMilliseconds(0);

           for(int i=0;i<10;i++){
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(i*100)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(20, TimeSpan.FromMilliseconds(i*100+50)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(i*100+100)));
           }

         
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(1000)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(100, TimeSpan.FromMilliseconds(1500)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(2000)));
           
           //fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(1500)));

          
           Storyboard.SetTarget(fi, winmain);
           Storyboard.SetTargetProperty(fi, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

           sb.Children.Add(fi);
           
           sb.RepeatBehavior = RepeatBehavior.Forever;

           // sb.AutoReverse = true;  
           sb.Begin();
          
       }

       public void StealFocus()
       {
           int x = (int)winmain.ActualWidth / 2, y = (int)winmain.ActualHeight / 2;
           SetCursorPos(x, y);
           mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);
           mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);
       }

        public void ShowHide()
       {
           if (WindowShowing)
           {
               MoveY(winmain, 0, -(int)winmain.Height, 500);
               CmdInvoker.CleanUp();
               StolenFocus = false;
           }
           else
           {
               MoveY(winmain, -(int)winmain.Height, 0, 500);
               winmain.Activate();
               txtCmdLine.Focus();
           }
           WindowShowing = !WindowShowing;
       }
        public void ShowAndHide()
        {
          
            Storyboard sb = new Storyboard();
        
            DoubleAnimationUsingKeyFrames fi = new DoubleAnimationUsingKeyFrames();
            fi.BeginTime = TimeSpan.FromMilliseconds(0);
          
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0,TimeSpan.FromMilliseconds(0)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(-500, TimeSpan.FromMilliseconds(500)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(-500, TimeSpan.FromMilliseconds(1000)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(1500)));

            sb.Children.Add(fi);
            Storyboard.SetTarget(sb, winmain);
            Storyboard.SetTargetProperty(sb, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
           
           // sb.AutoReverse = true;  
            sb.Begin();
            StolenFocus = false;
            winmain.Activate();
            txtCmdLine.Focus();
        }

       public void ShowHideOptions()
       {
           if (OptionsShowing)
           {
               OptionsWindow.UpdateValues();
               MoveX(OptionsWindow, 0, -(int)OptionsWindow.Width, 500);
               txtCmdLine.Focus();
           }
           else
           {
               OptionsWindow.UpdateValues();
               MoveX(OptionsWindow, -(int)OptionsWindow.Width, 0, 500);
               OptionsWindow.Activate();
           }
           OptionsShowing = !OptionsShowing;
       }

       private void SendCtrlC()
       {
           uint KEYEVENTF_KEYUP = 2;
           byte VK_CONTROL = 0x11;
           keybd_event(VK_CONTROL, 0, 0, 0);
           keybd_event(0x43, 0, 0, 0); //Send the C key (43 is "C")
           keybd_event(0x43, 0, KEYEVENTF_KEYUP, 0);
           keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);// 'Left Control Up

       }

       private void SendCtrlV()
       {
           uint KEYEVENTF_KEYUP = 2;
           byte VK_CONTROL = 0x11;
           keybd_event(VK_CONTROL, 0, 0, 0);
           keybd_event(0x56, 0, 0, 0); //Send the C key (43 is "C")
           keybd_event(0x56, 0, KEYEVENTF_KEYUP, 0);
           keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);// 'Left Control Up

       }

       private void GetSelectedText()
       {
           int activeWinPtr = GetForegroundWindow().ToInt32();
           int activeThreadId = 0, processId;
           activeThreadId = GetWindowThreadProcessId(activeWinPtr, out processId);
           int currentThreadId = GetCurrentThreadId();
           //String prev = txtCmdLine.Text;
           if (activeThreadId != currentThreadId)
           {
               AttachThreadInput(activeThreadId, currentThreadId, true);
               SendCtrlC();
               txtCmdLine.Text = "a ";
               txtCmdLine.CaretIndex = 2;
               txtCmdLine.Focus();
               SendCtrlV();
               AttachThreadInput(activeThreadId, currentThreadId, false);
               ShowHide();
               if (Clipboard.ContainsFileDropList())
               {
                   for (int i = 0; i < Clipboard.GetFileDropList().Count; i++)
                   {
                       GGResult r;
                       String f = new FileInfo(Clipboard.GetFileDropList()[i]).Name;
                       if (f.Contains('.')) f = f.Substring(0, f.LastIndexOf('.'));
                       r = CmdInvoker.InvokeCommand("a \"do " + f + "\"");
                       GGItem gg = CmdInvoker.GetGGList().GetGGItemAt(r.GetItemIndex());
                       HandleCmd(r);
                       r = CmdInvoker.InvokeCommand("pin " + (CmdInvoker.GetGGList().IndexOfGGItem(gg) + 1) + " " + Clipboard.GetFileDropList()[i]);
                       HandleCmd(r);
                   }
               }
               //if (txtCmdLine.Text.Equals("a ")) txtCmdLine.Text = prev;
           }
                           
       }

       public int HookDelegate(int nCode, int wParam, ref kbInfo lParam){
           if (nCode == 0)
           {
               kbInfo ki = (kbInfo)lParam;
               if (wParam == WM_KEYDOWN)
                   if (IsDown(VK_CONTROL) && ki.vkCode == vkKey(ShowHotkey))
                   {
                       GetSelectedText();
                   }
                   else if (ki.vkCode == vkKey(ShowHotkey))
                   {
                       if (InTutorialMode && RTBIndex == TUTORIAL_PAGE_NO - 1)
                       {
                           //showhide();
                           ShowAndHide();
                           DestroyTutorialMode();
                       }
                       else
                       {
                           ShowHide();
                           if (OptionsShowing) ShowHideOptions();
                       }
                   }
                   else if (ki.vkCode == vkKey(ExitHotkey))
                   {
                       CmdInvoker.CleanUp();
                       Application.Current.Shutdown();
                   }
                   else if (IsDown(VK_SHIFT) && ki.vkCode == vkKey(Key.F11))
                   {
                       if (WindowShowing) ShowHideOptions();
                   }
                   else if (ki.vkCode == vkKey(Key.F1))
                   {
                       if (WindowShowing && !InTutorialMode)
                       {
                           ShowAndHide();
                           DestroyMainMode();
                           InTutorialMode = true;
                       }
                   }
                   else if (WindowShowing && !StolenFocus)
                   {
                       StealFocus();
                       StolenFocus = true;
                   }

           }
           return CallNextHookEx(IntPtr.Zero, nCode, wParam, ref lParam);
       }

       public void CreateShortcut()
       {
           if (File.Exists(ShortcutLink)) return;
           IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
           IWshRuntimeLibrary.IWshShortcut ggshortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(ShortcutLink);
           ggshortcut.TargetPath = Assembly.GetExecutingAssembly().Location;
           ggshortcut.IconLocation = (new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName + "\\gg.ico";
           ggshortcut.WorkingDirectory = (new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName;
           ggshortcut.Description = "GG";
           
           ggshortcut.Save();
       }

       public void DeleteShortcut()
       {

           
           if(File.Exists(ShortcutLink))File.Delete(ShortcutLink);
          
       }

        public MainWindow()
        {
            InitializeComponent();
          
            hookDelegate = new deleHooky(HookDelegate);
            hookHandle=SetWindowsHookEx(WH_KEYBOARD_LL,hookDelegate,LoadLibrary("User32"),0);

            NotificationIcon = new System.Windows.Forms.NotifyIcon();
            NotificationIcon.Icon = new System.Drawing.Icon("gg.ico");
            NotificationIcon.Visible = true;
            NotificationIcon.Text = "GG is running. Press hotkey or click here to begin.";
            NotificationIcon.Click += new EventHandler(delegate(object o, EventArgs e)
            {
                ShowHide();
                if (OptionsShowing) ShowHideOptions();
            });
/*
            ni.BalloonTipClicked += new EventHandler(delegate(object o, EventArgs e)
            {
                showhide();
                if (showopt) showhideopt();
            });
  */          
          
           
            //   createshortcut();
            
        }

        public void DestroyAllRichTextBox()
        {
            for (int i = 0; i < RTBArray.Count; i++)
            {
                this.UnregisterName(RTBArray[i].Name);
                theGrid.Children.Remove(RTBArray[i]);
            }
            RTBArray.Clear();
            RTBArray = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private bool IsAddCommand(string text)
        {
            return Regex.IsMatch(text, ADD_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsListCommand(string text)
        {
            return Regex.IsMatch(text, LIST_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsChangeCommand(string text)
        {
            return Regex.IsMatch(text, CHANGE_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsRemoveCommand(string text)
        {
            return Regex.IsMatch(text, REMOVE_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsSyncCommand(string text)
        {
            return Regex.IsMatch(text, SYNC_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsPinCommand(string text)
        {
            return Regex.IsMatch(text, PIN_PATTERN, RegexOptions.IgnoreCase);
        }
        private bool IsActCommand(string text)
        {
            return Regex.IsMatch(text, ACT_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsUserTyping()
        {
            return CmdLineCaretIndex <= txtCmdLine.CaretIndex;
        }

        private void InitCommandLine()
        {
            if (txtCmdLine != null) return;
            txtCmdLine = new TextBox();
            txtCmdLine.Name = "cmdline";
            this.RegisterName(txtCmdLine.Name, txtCmdLine);
            Canvas.SetZIndex(txtCmdLine, 120);
            txtCmdLine.Height = 30;
            txtCmdLine.Width = RTBWidth;
           // txtcmdline.FontWeight = FontWeights.Bold;
            txtCmdLine.FontSize = 14;
            txtCmdLine.Padding = new Thickness(5,5,0,0);
            txtCmdLine.Background = new SolidColorBrush(Color.FromArgb(255,0, 0, 0));
            txtCmdLine.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            txtCmdLine.HorizontalAlignment = HorizontalAlignment.Left;
            txtCmdLine.Margin = new Thickness(RTBLeft, txtCmdLineTop, 0, 0);
            txtCmdLine.BorderThickness = new Thickness(0);
            txtCmdLine.KeyDown += cmdline_KeyDownEvent;
            

            txtCmdLine.TextChanged += delegate(Object sender, TextChangedEventArgs e)
            {
               
                if (IsUserTyping())
                {
                    if (IsAddCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "a | add THINGS I WANT TO DO";
                        FadeInAndOut(txtError);

                    }
                    else if (IsListCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "ls | list [TAG OR KEYWORD TO SEARCH]";
                        FadeInAndOut(txtError);

                    }
                    else if (IsChangeCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "ch | change";
                        FadeInAndOut(txtError);

                    }
                    else if (IsRemoveCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "rm | remove ITEMINDEX";
                        FadeInAndOut(txtError);

                    }
                    else if (IsSyncCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "sync USERID@gmail.com PASSWORD";
                        FadeInAndOut(txtError);
                    }
                    else if (IsPinCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "pin ITEMINDEX PATH | file | folder | none";
                        FadeInAndOut(txtError);
                    }
                    else if (IsActCommand(txtCmdLine.Text))
                    {
                        txtError.Text = "act ITEMINDEX";
                        FadeInAndOut(txtError);
                    }
                }
              
               CmdLineCaretIndex = txtCmdLine.CaretIndex;
            };
            theGrid.Children.Add(txtCmdLine);
        }

        private void InitErrorPopUp()
        {
            if (txtError != null) return;
            txtError = new TextBox();
            txtError.Name = "errtxt";
            this.RegisterName(txtError.Name, txtError);
            txtError.Height = 30;
            txtError.Width = RTBWidth/2;
            txtError.Padding = new Thickness(5, 3, 0, 0);
            txtError.BorderThickness = new Thickness(0);
            txtError.Clip = new RectangleGeometry(new Rect(new Point(0, 0), new Point(txtError.Width, txtError.Height)), 10, 10);
            txtError.Focusable = false;
            txtError.Text = "";
           //txtError.FontWeight = FontWeights.Bold;
            txtError.FontSize = 14;
            txtError.Background = new SolidColorBrush(Color.FromArgb(255,100,100,100));
            txtError.Opacity = 0;
            txtError.Foreground = white;
            txtError.HorizontalAlignment = HorizontalAlignment.Left;
            txtError.Margin = new Thickness(RTBLeft, txtErrorTop, 0, 0);
            theGrid.Children.Add(txtError);
        }

        private void InitBackground()
        {
            lblBackground = new Label();
            lblBackground.Name = "back";
            this.RegisterName(lblBackground.Name, lblBackground);


            lblBackground.Height = RTBHeight;
            lblBackground.Width = RTBWidth;
            lblBackground.Background = new SolidColorBrush(Colors.White);
            lblBackground.HorizontalAlignment = HorizontalAlignment.Left;
            lblBackground.Margin = new Thickness(RTBLeft, RTBTop, 0, 0);

            theGrid.Children.Add(lblBackground);
        }

        private void InitNextPrevTagTxtBox()
        {
            if (txtNextArrow != null && txtPrevArrow != null) return;
            txtNextArrow = new TextBox();
            txtPrevArrow = new TextBox();
           
            txtNextArrow.Name = "next";
            txtPrevArrow.Name = "prev";
          

            this.RegisterName(txtNextArrow.Name, txtNextArrow);
            this.RegisterName(txtPrevArrow.Name, txtPrevArrow);
          
            Canvas.SetZIndex(txtNextArrow, 110);
            Canvas.SetZIndex(txtPrevArrow, 110);

         

            txtNextArrow.Height = 40;
            txtNextArrow.Width = 40;

            txtPrevArrow.Height =40;
            txtPrevArrow.Width = 40;

          
            txtNextArrow.Margin = new Thickness(RTBLeft + RTBWidth - 35, RTBHeight - 20, 0, 0);
            txtPrevArrow.Margin = new Thickness(RTBLeft -5, RTBHeight - 20, 0, 0);

          

            txtNextArrow.HorizontalAlignment = HorizontalAlignment.Left;
            txtPrevArrow.HorizontalAlignment = HorizontalAlignment.Left;
            txtNextArrow.VerticalAlignment = VerticalAlignment.Top;
            txtPrevArrow.VerticalAlignment = VerticalAlignment.Top;   

            SolidColorBrush trans = new SolidColorBrush(Colors.Transparent);

            txtNextArrow.Background = trans;
            txtPrevArrow.Background = trans;

           
            txtNextArrow.TextAlignment = TextAlignment.Center;
            txtPrevArrow.TextAlignment = TextAlignment.Center;

            
           txtNextArrow.BorderThickness = new Thickness(0);
           txtPrevArrow.BorderThickness = new Thickness(0);

           
            
            
            txtNextArrow.Text = ">";
            txtPrevArrow.Text = "<";


            txtNextArrow.Foreground = black;//new SolidColorBrush(Colors.White);
           txtPrevArrow.Foreground = black;//new SolidColorBrush(Colors.White);
          
            
            txtNextArrow.FontFamily = Default_Arrow_Font;
            txtPrevArrow.FontFamily = Default_Arrow_Font;

           
            
            txtNextArrow.FontWeight = FontWeights.Bold;
            txtPrevArrow.FontWeight = FontWeights.Bold;

            
            txtNextArrow.FontSize = 20;
            txtPrevArrow.FontSize = 20;

             txtNextArrow.Focusable = false;
            

             txtPrevArrow.Focusable = false;
            
            theGrid.Children.Add(txtPrevArrow);
            theGrid.Children.Add(txtNextArrow);
        }

        private void NewRichTextBox()
        {
            int i = RTBArray.Count();
            RTBArray.Add(new RichTextBox());
            RTBArray[i].Name = "txt" + RTBId++;
            this.RegisterName(RTBArray[i].Name, RTBArray[i]);

            RTBArray[i].RenderTransform = new TranslateTransform();
            RTBArray[i].Height = RTBHeight;
            RTBArray[i].Width = RTBWidth;
            RTBArray[i].Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            RTBArray[i].HorizontalAlignment = HorizontalAlignment.Left;
            RTBArray[i].Margin = new Thickness(RTBLeft, RTBTop, 0, 0);
            RTBArray[i].Focusable = false;
           
            RTBArray[i].BorderThickness = new Thickness(0);

            RTBArray[i].FontFamily = Default_Font;
            RTBArray[i].Document.LineHeight = 5;

            Canvas.SetZIndex(RTBArray[i], 0);
            theGrid.Children.Add(RTBArray[i]);
            MoveX(RTBArray[i], 0, -txtShift, 0);
        }

        private void InitAllTextBoxes()
        {
            RTBArray = new List<RichTextBox>();
            DeletedTagLists =  new List<bool>();
            for (int i = 0; i < TEXTBOX_NO; i++)
            {
                NewRichTextBox();
                DeletedTagLists.Add(false);
            }
            RTBNow = RTBArray[TEXTBOX_NO-1];
            RTBIndex = TEXTBOX_NO-1;
            MoveX(RTBArray[TEXTBOX_NO-1], -txtShift, 0, 0);

            InitCommandLine();
            InitErrorPopUp();

   
        }

        private void TutorialMode()
        {
            RTBArray = new List<RichTextBox>();
            DeletedTagLists = new List<bool>();
            for (int i = 0; i < TUTORIAL_PAGE_NO; i++)
            {
                NewRichTextBox();
                DeletedTagLists.Add(false);
            }
            RTBNow = RTBArray[0];
            RTBIndex = 0;
            MoveX(RTBArray[0], -txtShift, 0, 0);

            InitCommandLine();
            InitErrorPopUp();
        }

        private void LoadTutorialPages()
        {
            for (int i = 0; i < TUTORIAL_PAGE_NO; i++)
            {
                RTBArray[i].Document.Blocks.Clear();
                Image img =  new Image();
                img.Source=new BitmapImage(new Uri((new FileInfo(Assembly.GetExecutingAssembly().Location)).DirectoryName+"\\tut\\"+i+".png"));
                img.Width = RTBWidth;
                if (i == 3) img.Height = RTBHeight*2;
                    else img.Height = RTBHeight;
                RTBArray[i].Document.Blocks.Add(new BlockUIContainer(img));
            }

        }

        private void DestroyTutorialMode()
        {
            DestroyAllRichTextBox();
            InitAllTextBoxes();
            RefreshContents(RTBIndex);
            InTutorialMode = false;
        }

        private void DestroyMainMode()
        {
            DestroyAllRichTextBox();
            TutorialMode();
            LoadTutorialPages();
        }

        private void InitOptions()
        {
            if (!File.Exists(SettingsFile))
            {
                OptionsObject opt = new OptionsObject();
                opt.RunOnStartup = RunOnStartup;
                opt.ShowHotkey = ShowHotkey;
                opt.ExitHotkey = ExitHotkey;
                opt.ScrollHotkey = ScrollHotkey;
                OptionsIO optio = new OptionsIO(SettingsFile);
                optio.Write(opt);
                InTutorialMode = true;
            }
            else
            {
                OptionsObject opt = new OptionsIO(SettingsFile).Read();
                ShowHotkey = opt.ShowHotkey;
                ExitHotkey = opt.ExitHotkey;
                ScrollHotkey = opt.ScrollHotkey;
                RunOnStartup = opt.RunOnStartup;
                InTutorialMode = false;
            }

            if (!RunOnStartup) DeleteShortcut();
            else CreateShortcut();

            OptionsWindow = new options(winmain);
            OptionsWindow.Show();
            MoveX(OptionsWindow, 0, -(int)OptionsWindow.Width, 0);

            //tutorialmode = true;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            winmain.Top = 0;
            winmain.Left = SystemParameters.PrimaryScreenWidth * .05;
            winmain.Width = SystemParameters.PrimaryScreenWidth * .75;
            winmain.Height = SystemParameters.PrimaryScreenHeight/2;

            winmain.RenderTransform = new TranslateTransform();
            MoveY(winmain, 0, -(int)winmain.Height, 0);

            winmain.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            RTBWidth = winmain.Width * .8;
            RTBHeight = winmain.Height * .7;
            RTBTop = -winmain.Height * .25;
            RTBLeft = 20;
            txtCmdLineTop = winmain.Height * .525;
            txtErrorTop = winmain.Height * .70;
            txtShift = (int)RTBWidth;


            RectangleGeometry s = new RectangleGeometry(new Rect(new Point(RTBLeft, FrameTop+10), new Point(RTBLeft + RTBWidth, FrameTop + RTBHeight+40/*winmain.Height*/)),6,6);

            Geometry g = new CombinedGeometry(s, new RectangleGeometry(new Rect(new Point(RTBLeft, FrameTop + RTBHeight + 40), new Point(RTBLeft + RTBWidth / 2, FrameTop + RTBHeight + 100))));

            theGrid.Clip = g;

            FrameTop = winmain.Height * .025;
            WindowShowing = false;

            HistoryList = new List<String>();
            BeginLogic();
           InitBackground();
           InitOptions();
           if (InTutorialMode)
           {
               TutorialMode();
               LoadTutorialPages();
               ShowHide();
           }
           else
           {
               InitAllTextBoxes();
               RefreshContents(RTBIndex);
               
           }

           InitNextPrevTagTxtBox();
         
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            UnhookWindowsHookEx(hookHandle);
            NotificationIcon.Visible = false;
            NotificationIcon.Dispose();
        }

        private static void MoveX(UIElement obj, int start, int end, int dur)
        {
            DoubleAnimation datxt = new DoubleAnimation();
            datxt.Duration = new Duration(TimeSpan.FromMilliseconds(dur));
            datxt.From = start;
            datxt.To = end;
            datxt.EasingFunction = new QuarticEase();
            TranslateTransform tt = (TranslateTransform)obj.RenderTransform;
            tt.BeginAnimation(TranslateTransform.XProperty, datxt);


        }

        private static void MoveY(UIElement obj, int start, int end, int dur)
        {
            DoubleAnimation datxt = new DoubleAnimation();
            datxt.Duration = new Duration(TimeSpan.FromMilliseconds(dur));
            datxt.From = start;
            datxt.To = end;
            datxt.EasingFunction = new QuarticEase();


            TranslateTransform tt = (TranslateTransform)obj.RenderTransform;
            tt.BeginAnimation(TranslateTransform.YProperty, datxt);


        }

        private static void FadeInAndOut(UIElement obj)
        {
          
            Storyboard sb = new Storyboard();
        
            DoubleAnimationUsingKeyFrames fi = new DoubleAnimationUsingKeyFrames();
            fi.BeginTime = TimeSpan.FromMilliseconds(0);
          
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0,TimeSpan.FromMilliseconds(0)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromMilliseconds(500)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromMilliseconds(3000)));
           fi.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromMilliseconds(4000)));


            sb.Children.Add(fi);
            Storyboard.SetTarget(sb, obj);
            Storyboard.SetTargetProperty(sb, new PropertyPath(Control.OpacityProperty));
           
           // sb.AutoReverse = true;  
            sb.Begin();
            
            

        }


        private int GetNextIndex()
        {
            return (RTBIndex+1)%(InTutorialMode?TUTORIAL_PAGE_NO:TEXTBOX_NO);
        }

        private int GetPrevIndex()
        {
            return (RTBIndex - 1) < 0 ? (InTutorialMode ? TUTORIAL_PAGE_NO : TEXTBOX_NO) - 1 : RTBIndex - 1;
        }

        private int GetTagIndex(string tagtofind)
        {
            return TagList.FindIndex(delegate(string tag)
            {
                return tagtofind.CompareTo(tag) == 0;
            });
        }

        private void PurgeDeletedLists()
        {
            for (int i = 0; i < RTBArray.Count; i++)
                if (DeletedTagLists[i])
                {
                    theGrid.Children.Remove(RTBArray[i]);
                    this.UnregisterName(RTBArray[i].Name);
                    RTBArray.RemoveAt(i);
                    RTBNow = RTBArray[RTBIndex];

                    DeletedTagLists.RemoveAt(i);
                }
        }

        private void txtbox_PreviewKeyDownEvent(object e, KeyboardEventArgs k)
        {
            int inscreen = 0, rexit = txtShift, lexit = -txtShift, duration = 1000;

            if (IsDown(vkKey(ScrollHotkey)) && k.KeyboardDevice.IsKeyDown(Key.Right))
            {
                MoveX(RTBNow, inscreen, lexit, duration);

                PurgeDeletedLists();
                
                RTBIndex = GetNextIndex();
                RTBNow = RTBArray[RTBIndex];


                MoveX(RTBNow, rexit, inscreen, duration);
                if(!InTutorialMode)RefreshContents(RTBIndex);


            }
            else if (IsDown(vkKey(ScrollHotkey)) && k.KeyboardDevice.IsKeyDown(Key.Left))
            {

                MoveX(RTBNow, inscreen, rexit, duration);
                PurgeDeletedLists();
              
                RTBIndex = GetPrevIndex();
                RTBNow = RTBArray[RTBIndex];


                MoveX(RTBNow, lexit, inscreen, duration);

                if (!InTutorialMode) RefreshContents(RTBIndex);                
                
            }
            else if (IsDown(vkKey(ScrollHotkey)) && k.KeyboardDevice.IsKeyDown(Key.Down))
            {
                RTBNow.LineDown();
              
              
            }
            else if (IsDown(vkKey(ScrollHotkey)) && k.KeyboardDevice.IsKeyDown(Key.Up))
            {
                RTBNow.LineUp();
              
            }
            else if (k.KeyboardDevice.IsKeyDown(Key.Down))
            {
                if (HistoryList.Count > 0)
                {
                    HistoryListIndex = (HistoryListIndex - 1) < 0 ? HistoryList.Count - 1 : HistoryListIndex - 1;
                    txtCmdLine.Text = HistoryList[HistoryListIndex];
                    txtCmdLine.CaretIndex = txtCmdLine.Text.Length;
                }
            }
            else if (k.KeyboardDevice.IsKeyDown(Key.Up))
            {
                if (HistoryList.Count > 0)
                {
                    txtCmdLine.Text = HistoryList[HistoryListIndex];
                    HistoryListIndex = (HistoryListIndex + 1) % HistoryList.Count;
                    txtCmdLine.CaretIndex = txtCmdLine.Text.Length;
                }
            }
           
        }

        private bool IsExitCommand(string userCommand)
        {
            if (Regex.IsMatch(userCommand.Trim(), "^exit\\s*$", RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleAddDelete(GGResult result)
        {
           // RefreshContents(nowindex);
            if (result.GetResultType() == GGResult.RESULT_TYPE.RESULT_ADD)
            {
                if (CmdInvoker.GetTagCount(result.GetTag()) == 1)
                {
                    TEXTBOX_NO++;
                    TagList.Add(result.GetTag());
                    DeletedTagLists.Add(false);
                    NewRichTextBox();
                  
                }
            }
            else if(result.GetResultType() == GGResult.RESULT_TYPE.RESULT_REMOVE) //delete
            {
                if (CmdInvoker.GetTagCount(result.GetTag()) == -1)
                {
                    TEXTBOX_NO--;
                    int index = GetTagIndex(result.GetTag());
                    DeletedTagLists[index] = true;
                    TagList.RemoveAt(index);
                    RTBIndex = GetPrevIndex();
                    RTBIndex = GetNextIndex();
              
           
                }
            }
        }

        private void HandleUpdate(GGResult result)
        {
           // RefreshContents(nowindex);
            string oldtag = result.GetTag();
            string newtag = CmdInvoker.GetGGList().GetGGItemAt(result.GetItemIndex()).GetTag();
            if (CmdInvoker.GetTagCount(newtag) == 1)
            {
                TEXTBOX_NO++;
                TagList.Add(newtag);
                DeletedTagLists.Add(false);
                NewRichTextBox();
            }
            if (CmdInvoker.GetTagCount(oldtag) == -1)
            {
                TEXTBOX_NO--;
                int index = GetTagIndex(result.GetTag());
                DeletedTagLists[index] = true;
                TagList.RemoveAt(index);
                RTBIndex = GetPrevIndex();
                RTBIndex = GetNextIndex();
            }


        }

        private void HandleList(GGResult result)
        {
            // command returns something
            Table t = new Table();
            t.Columns.Add(new TableColumn());
            t.Columns.Add(new TableColumn());
            t.Columns.Add(new TableColumn());

            double tablewidth = RTBWidth * (13.2 / 16.0);


            t.Columns[0].Width = new GridLength(tablewidth / 2);
            t.Columns[1].Width = new GridLength(tablewidth / 4);
            t.Columns[2].Width = new GridLength(tablewidth / 4);


            t.BorderThickness = new Thickness(RTBWidth / 16, 50, 0, 0);
            t.CellSpacing = 10;
            t.RowGroups.Add(AddTextChunkToTable(result.GetMainResult()));
            RTBNow.Document.Blocks.Add(t);
        }

        private void HandleSync()
        {
            DestroyAllRichTextBox();
            InitTagList();
            InitAllTextBoxes();
            RefreshContents(RTBIndex);
        }

        private void cmdline_KeyDownEvent(object e, KeyboardEventArgs k)
        {
            if (!InTutorialMode && k.KeyboardDevice.IsKeyDown(Key.Enter))
            {
                if (IsExitCommand(txtCmdLine.Text))
                {
                    CmdInvoker.CleanUp();
                    Application.Current.Shutdown();
                }
                else if (txtCmdLine.Text.StartsWith("wiggle"))
                {
                    wiggle();
                    txtCmdLine.Text = "";
                    return;
                }

                GGResult result = CmdInvoker.InvokeCommand(txtCmdLine.Text);
                HandleCmd(result);
                /*               
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += BW_DoWork;
                bw.RunWorkerCompleted += BW_Completed;
                bw.RunWorkerAsync(txtCmdLine.Text);
                HistoryList.Add(txtCmdLine.Text);*/
                if (HistoryList.Count > 10) HistoryList.RemoveAt(0);
                HistoryListIndex = 0;
                txtCmdLine.Text = "";
            }
          
        }

        
        public void HandleCmd(GGResult result)
        {
            txtError.Clear();
            RTBNow.Document.Blocks.Clear();

            if (result.GetResultType() == GGResult.RESULT_TYPE.RESULT_LIST)
            {
                HandleList(result);
            }
            else if (result.GetResultType() == GGResult.RESULT_TYPE.RESULT_ADD || result.GetResultType() == GGResult.RESULT_TYPE.RESULT_REMOVE)
            {
                HandleAddDelete(result);
            }
            else if (result.GetResultType() == GGResult.RESULT_TYPE.RESULT_UPDATE)
            {
                HandleUpdate(result);
            }
            else if (result.GetResultType() == GGResult.RESULT_TYPE.RESULT_SYNC)
            {
                HandleSync();
            }


            if (result.GetResultType() != GGResult.RESULT_TYPE.RESULT_LIST) RefreshContents(RTBIndex);

            txtError.Text = result.GetNotice();
            if (result.GetNotice() != String.Empty) FadeInAndOut(txtError);
            HistoryList.Add(txtCmdLine.Text);
            if (HistoryList.Count > 10) HistoryList.RemoveAt(0);
            HistoryListIndex = 0;
            txtCmdLine.Text = "";
        }

        private void BW_DoWork(object sender,DoWorkEventArgs e)
        {
            e.Result = CmdInvoker.InvokeCommand((string)e.Argument);
        }

        void BW_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
 
            HandleCmd((GGResult)e.Result);
           
        }


    }
}
       