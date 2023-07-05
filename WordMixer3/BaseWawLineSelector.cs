using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WordMixer3 {
    internal class BaseWawLineSelector{
        private int start=0, end=0;
        public Rectangle rect;
        public Rectangle nrect;
        public bool changingLeft = true;

        public int min { get { return start > end ? end : start; } }
        public int max { get { return start < end ? end : start; } }
        public int length { get { return max - min + 1; } }

        public BaseWawLineSelector() {
            rect = new Rectangle();
            rect.Height = 100;
            rect.Fill = new SolidColorBrush(Color.FromArgb(50,255,255,0));
            nrect = new Rectangle();
            nrect.Height = 100;
            nrect.Width = 1;
            nrect.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 0));
        }
        public void changing(int x) {
            if (changingLeft) start = x;
            else end = x;
        }
        public void affect(int x) {
            if (changingLeft) start += x;
            else end += x;
        }
        public void setStart(int x) {
            this.start = x;
        }
        public void setEnd(int x) {
            this.end = x;
        }
    }
}
