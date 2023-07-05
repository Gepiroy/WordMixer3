using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMixer3 {
    public static class SoundManager {
        public static List<SoundAtLoc> sounds = new List<SoundAtLoc>();

        static LinerFile lf;

        public static void init() {
            lf = new LinerFile(App.basePath + "sounds.txt", true);
            foreach (string st in lf.sts) {
                addSound(new SoundAtLoc(st));
            }
        }

        /*public static SoundAtLoc findOrCreate(int from, int to) {
            foreach (SoundAtLoc s in sounds) {
                if (s.from == from && s.to == to) return s;
            }
            return new SoundAtLoc(from, to);
        }*/

        public static void save() {
            lf.sts.Clear();
            foreach (SoundAtLoc s in sounds) {
                lf.sts.Add(s.ToString());
            }
            lf.save();
        }

        public static SoundAtLoc salo = new SoundAtLoc(0, 0); //Наш ИЗБРАННЫЙ отрезок!

        public static void setSalo(SoundAtLoc sal) {
            tryToAddSal(salo);//save old salo
            salo = sal;
            if (sal == null) {
                throw new ArgumentNullException();
            } else {
                updateVidget();
            }
        }
        public static void updateVidget() {
            MainWindow.instance.sound.Text = salo.sound;
            MainWindow.instance.startPC.setValue(salo.from);
            MainWindow.instance.startPC.onChange = (v) => { salo.setFrom(v); MainWindow.instance.bowl.updateSelection(); };
            MainWindow.instance.endPC.setValue(salo.to);
            MainWindow.instance.endPC.onChange = (v) => { salo.setTo(v); MainWindow.instance.bowl.updateSelection(); };
        }
        public static void tryToAddSal(SoundAtLoc sal) {
            if (sal != null && !sounds.Contains(sal) && !sal.sound.Equals("~")) {
                addSound(sal);
            }
        }
        public static void setSalo(int from, int to) {
            setSalo(new SoundAtLoc(from, to));
        }

        static void addSound(SoundAtLoc s) {
            s.createElements();
            sounds.Add(s);
            if (MainWindow.instance != null) {
                MainWindow.instance.bowl.canv.Children.Add(s.rect);
                MainWindow.instance.bowl.canv.Children.Add(s.tb);
                MainWindow.instance.bowl.draw();
            }
        }

        public static void playSalo() {
            Audio.play(Audio.getPiece(salo.from, salo.to-salo.from+1));
        }

        public static SoundAtLoc soundAtFrame(int frame) {
            foreach (SoundAtLoc sal in sounds) {
                if (sal.from <= frame && sal.to >= frame) return sal;
            }
            return null;
        }
    }
}
