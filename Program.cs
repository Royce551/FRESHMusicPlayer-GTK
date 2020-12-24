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

            // To force the system borders, although it will be very ugely on gnome, you do this
            // Environment.SetEnvironmentVariable("GTK_CSD", "0");
            // And then of course, do the headerbar thin seperately

            var app = new Application("org.FRESHMusicPlayer_GTK.FRESHMusicPlayer_GTK", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
