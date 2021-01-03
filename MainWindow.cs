using ATL;
using System;
using System.Timers;
using Gtk;
using System.IO;
using UI = Gtk.Builder.ObjectAttribute;
using LiteDB;
using FRESHMusicPlayer.Handlers;

namespace FRESHMusicPlayer
{
    partial class MainWindow : Window
    {
        Player player = new();
        Timer progressTimer = new Timer(1000);
        Track currentTrack;
        LiteDatabase library = new LiteDatabase(System.IO.Path.Combine(DatabaseHandler.DatabasePath, "database.fdb2"));
        DatabaseUtils databaseUtils;

        const string windowName = "FRESHMusicPlayer GTK";

        public MainWindow() : this(new Builder("MainWindow.glade")) 
        {
            
        }

        // Controls
        private void VolumeBar_ValueChanged(object sender, EventArgs e)
        {
            if (player.Playing)
            {
                player.CurrentVolume = (float)VolumeBar.Value;
                player.UpdateSettings();
            }
        }

        private void Player_SongException(object sender, Handlers.PlaybackExceptionEventArgs e)
        {
            // ignored for now
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
            SeekBar.SetRange(1, player.CurrentBackend.TotalTime.TotalSeconds);
            if (player.CurrentBackend.TotalTime.TotalSeconds != 0) ProgressLabel2.Text = player.CurrentBackend.TotalTime.ToString(@"mm\:ss");
            else ProgressLabel2.Text = "∞";
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
            var time = TimeSpan.FromSeconds(Math.Floor(player.CurrentBackend.CurrentTime.TotalSeconds));
            ProgressLabel1.Text = time.ToString(@"mm\:ss");
            //SeekBar.Value = time.TotalSeconds;
            player.AvoidNextQueue = false;
        }
        // Import Tab
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
        // Tracks Tab

        // Other
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
    }
}
