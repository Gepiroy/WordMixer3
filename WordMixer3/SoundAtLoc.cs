using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WordMixer3 {
    public class SoundAtLoc {
        public int from, to;
        public int length { get { return to - from + 1; } }
        public string sound;
        public TextBlock tb;
        public Rectangle rect;

        public SoundAtLoc(string line) {
            string[] sts = line.Split('-');
            from = int.Parse(sts[0]);
            string[] sts2 = sts[1].Split(':');
            to=int.Parse(sts2[0]);
            sound = sts2[1];
            createElements();
        }

        public SoundAtLoc(int from, int to) {
            this.from = from;
            this.to = to;
            this.sound = "~";
            createElements();
        }

        public void setFrom(int s) {
            from = s;
            updateDisplays();
        }

        public void updateDisplays() {
            rect.Width = BaseWavLine.ppf * length;
            Elements.setPosAtCanvas(rect, from * BaseWavLine.ppf, 10);
            Elements.setPosAtCanvas(tb, from * BaseWavLine.ppf, 0);
        }

        public void setTo(int s) {
            to = s;
            updateDisplays();
        }

        public void createElements() {
            tb = new TextBlock { Text = sound, FontSize=8 };
            rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));
            rect.Height = 3;
        }

        public override string ToString() {
            return from + "-" + to + ":" + sound;
        }
    }
}
