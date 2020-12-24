using ATL;
using System;
using System.Timers;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace FRESHMusicPlayer
{
    class MainWindow : Window
    {
        Player player = new();
        Timer progressTimer = new Timer(1000);
        Track currentTrack;

        [UI] Button BrowseTracksButton = null;

        [UI] Label TitleLabel = null;
        [UI] Label ArtistLabel = null;

        [UI] Button PreviousButton = null;
        [UI] ToggleButton ShuffleButton = null;
        [UI] Button PlayPauseButton = null;
        [UI] ToggleButton RepeatOneButton = null;
        [UI] Button NextButton = null;

        const string windowName = "FRESHMusicPlayer GTK";

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            progressTimer.Elapsed += Timer_Elapsed;
            player.SongChanged += Player_SongChanged;
            player.SongStopped += Player_SongStopped;
            player.SongException += Player_SongException;
            Title = windowName;
            BrowseTracksButton.Clicked += BrowseTracksButton_Clicked;

            NextButton.Clicked += NextButton_Clicked;
            RepeatOneButton.Clicked += RepeatOneButton_Clicked;
            PlayPauseButton.Clicked += PlayPauseButton_Clicked;
            ShuffleButton.Clicked += ShuffleButton_Clicked;
            PreviousButton.Clicked += PreviousButton_Clicked;
            DeleteEvent += Window_DeleteEvent;
        }

        private void Player_SongException(object sender, Handlers.PlaybackExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Player_SongStopped(object sender, EventArgs e)
        {
            Title = windowName;
            TitleLabel.Text = ArtistLabel.Text = "Nothing Playing";
            progressTimer.Stop();
        }

        private void Player_SongChanged(object sender, EventArgs e)
        {
            currentTrack = new Track(player.FilePath);
            Title = $"{currentTrack.Artist} - {currentTrack.Title} | {windowName}";
            TitleLabel.Text = currentTrack.Title;
            ArtistLabel.Text = currentTrack.Artist == string.Empty ? "No artist" : currentTrack.Artist;
            progressTimer.Start();
        }

        private void PreviousButton_Clicked(object sender, EventArgs e) => PreviousTrackMethod();

        private void ShuffleButton_Clicked(object sender, EventArgs e) => player.Shuffle = ShuffleButton.Active;

        private void PlayPauseButton_Clicked(object sender, EventArgs e) => PlayPauseMethod();

        private void RepeatOneButton_Clicked(object sender, EventArgs e) => player.RepeatOnce = RepeatOneButton.Active;

        private void NextButton_Clicked(object sender, EventArgs e) => NextTrackMethod();

        public void PlayPauseMethod()
        {
            if (!player.Playing) return;
            if (player.Paused)
            {
                player.ResumeMusic();
                progressTimer.Start();
            }
            else
            {
                player.PauseMusic();
                progressTimer.Stop();
            }
        }
        public void StopMethod()
        {
            player.ClearQueue();
            player.StopMusic();
        }
        public void NextTrackMethod() => player.NextSong();
        public void PreviousTrackMethod() => player.PreviousSong();
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
           
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
            progressTimer.Start();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
    }
}
