using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace WordMixer3 {
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application {
        public static readonly int fps = 25;
        public static Random r = new Random();

        public static string basesPath;
        public static string basePath;
        protected override void OnStartup(StartupEventArgs e) {
            basesPath = System.AppDomain.CurrentDomain.BaseDirectory + "bases\\";
            LinerFile f = new LinerFile(basesPath+"choosen.txt", false);
            if (f.sts.Count == 0) {
                f.sts.Add("defaultBase");
                f.save();
            }
            basePath = basesPath + f.sts[0]+"\\";

            Directory.CreateDirectory(basePath);

            Audio.loadWav(basePath+"voice.wav");

            SoundManager.init();
        }

        private void onExit(object sender, ExitEventArgs e) {
            SoundManager.save();
        }

        void genBred() {
            List<byte> toPlay = new List<byte>();
            for (int i = 0; i < 50; i++) {
                toPlay.AddRange(Audio.getPiece(r.Next(Audio.totalFrames - 4), r.Next(3) + 2));
            }
            //Audio.render(basePath+"output.wav", toPlay.ToArray());
            Audio.play(toPlay.ToArray());
        }
    }
}
