using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace FRESHMusicPlayer
{
    class MainWindow : Window
    {
        Player player = new Player();
        [UI] Button BrowseTracksButton = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            BrowseTracksButton.Clicked += BrowseTracksButton_Clicked;
            DeleteEvent += Window_DeleteEvent;
        }

        private void BrowseTracksButton_Clicked(object sender, EventArgs e)
        {
            using var openFileDialog1 = new FileChooserDialog("Choose file",
                                                              this,
                                                              FileChooserAction.Open,
                                                              "Cancel", ResponseType.Cancel,
                                                              "OK", ResponseType.Accept);
            var result = (ResponseType)openFileDialog1.Run();
            if (result != ResponseType.Accept) return;
            player.CurrentVolume = 0.7f;
            player.AddQueue(openFileDialog1.Filename);
            player.PlayMusic();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

    }
}
