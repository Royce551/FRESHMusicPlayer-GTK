using System;
using FRESHMusicPlayer;
using Gtk;

namespace FRESHMusicPlayer
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.FRESHMusicPlayer_GTK.FRESHMusicPlayer_GTK", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
