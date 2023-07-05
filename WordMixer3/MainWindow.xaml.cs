using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordMixer3 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow instance;
        public MainWindow() {
            InitializeComponent();
            instance = this;
        }

        private void stc(object sender, TextChangedEventArgs e) {//SoundTextChanged
            SoundManager.salo.sound = sound.Text;
        }

        private void rselClick(object sender, RoutedEventArgs e) {
            int from = App.r.Next(Audio.totalFrames - 9);
            int to=from+2+App.r.Next(7);
            bowl.select(from, to);
            SoundManager.playSalo();
        }

        private void playSelected(object sender, RoutedEventArgs e) {
            SoundManager.playSalo();
        }

        private void addSelected(object sender, RoutedEventArgs e) {
            mix.addSound(new MixSound(sound.Text, SoundManager.salo));
        }

        private void playMix(object sender, RoutedEventArgs e) {
            Audio.play(mix.getBytes());
        }

        private void renderMix(object sender, RoutedEventArgs e) {
            Audio.render(App.basePath+"out.wav",mix.getBytes());
        }

        private void gen(object sender, RoutedEventArgs e) {
            mix.genMix(toGen.Text);
        }

        private void leaveSAL(object sender, RoutedEventArgs e) {

        }

        private void kDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                    
                    break;
                case Key.Right:
                    break;
                case Key.Space:
                    break;
            }
        }
    }
}
