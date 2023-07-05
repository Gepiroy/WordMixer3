using NAudio.CoreAudioApi;
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
    /// Логика взаимодействия для BaseWavLine.xaml
    /// </summary>
    public partial class BaseWavLine : UserControl {
        private BaseWawLineSelector sel = new BaseWawLineSelector();

        double startX = 0;

        public BaseWavLine() {
            InitializeComponent();
            resetRects();
            canv.Children.Add(sel.rect);
            canv.Children.Add(sel.nrect);
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            draw();
        }
        public static double ppf=0;
        Rectangle[] disFrames = new Rectangle[0];

        public void resetRects() {
            foreach (Rectangle rect in disFrames) {
                canv.Children.Remove(rect);
            }
            if (!Audio.isLoaded) return;
            disFrames = new Rectangle[Audio.totalFrames];
            for (int i = 0; i < Audio.totalFrames; i++) {
                Rectangle rect = new Rectangle();
                rect.Height = 50;
                rect.Width = 2;
                byte g = (byte)(i % 2 * 100 + Math.Abs(50 - i % 200 / 2));
                rect.Fill = new SolidColorBrush(Color.FromRgb(g, g, g));
                disFrames[i] = rect;
                canv.Children.Add(rect);
            }
            foreach (SoundAtLoc s in SoundManager.sounds) {
                canv.Children.Add(s.rect);
                canv.Children.Add(s.tb);
            }
        }

        public void draw() {
            if (ActualWidth <= 0 || ActualHeight <= 0 || double.IsNaN(ActualHeight) || double.IsNaN(ActualWidth)) return;
            if (!Audio.isLoaded) return;
            if(ppf==0)ppf = ActualWidth / Audio.totalFrames;
            updatePpf();
        }

        int mToX(double mp) {
            return (int)(mp / ppf);
        }
        private bool down = false, changing=false;
        Action<MouseButtonEventArgs> md;
        private void mDown(object sender, MouseButtonEventArgs e) {
            Keyboard.Focus(this);
            int f = mToX(e.GetPosition(canv).X);
            double y = e.GetPosition(canv).Y;
            if (y < 13) {//Зона над дорожками

            } else {//Дорожки
                if(md!=null)md(e);
            }
            down = true;
            mMove(null, e);
        }

        private void mUp(object sender, MouseButtonEventArgs e) {
            int f = mToX(e.GetPosition(canv).X);
            double y = e.GetPosition(canv).Y;
            if (y < 13) {//Зона над дорожками
                SoundAtLoc sal = SoundManager.soundAtFrame(f);
                if (sal!=null) {
                    select(sal.from, sal.to);
                    SoundManager.setSalo(sal);
                    Audio.play(Audio.getPiece(Math.Max(0, sel.min), sel.length));
                }
            } else {//Дорожки
                if(!changing)SoundManager.setSalo(Math.Max(0, sel.min), sel.max);
                else {
                    setSaloBySelection();
                }
                Audio.play(Audio.getPiece(Math.Max(0, sel.min), sel.length));
            }
            changing = false;
            down = false;
        }

        void setSaloBySelection() {
            SoundManager.salo.from = sel.min;
            SoundManager.salo.to = sel.max;
            SoundManager.salo.updateDisplays();
            SoundManager.updateVidget();
        }

        private void mMove(object sender, MouseEventArgs e) {
            Point p = e.GetPosition(canv);
            int f = mToX(p.X);
            double y = p.Y;
            Cursor c = Cursors.Arrow;
            if (down&& y >= 13) {
                sel.changing(f);
                //sel.setEnd(f);
                redrawSelection();
            } else {
                if (y < 13) {//Зона над дорожками
                    if (SoundManager.soundAtFrame(f)!=null) {
                        c = Cursors.Hand;
                    }
                } else {//Дорожки
                    double distStart = Math.Abs(sel.min * ppf - p.X);
                    double distEnd = Math.Abs((sel.max + 1) * ppf - p.X);
                    if (distStart <= 5 || distEnd <= 5) {
                        md = (ev) => {
                            if (distStart < distEnd) {
                                sel.changingLeft = true;
                            } else {
                                sel.changingLeft = false;
                            }
                            changing = true;
                        };
                        c = Cursors.SizeWE;
                    } else {
                        md = (ev) => {
                            sel.setStart(f);
                            sel.setEnd(f);
                        };
                    }
                }

            }
            Mouse.SetCursor(c);
        }
        public void select(int from, int to) {//Выбрать с заменой сала.
            sel.setStart(from);
            sel.setEnd(to);
            redrawSelection();
            SoundManager.setSalo(sel.min, sel.max);
        }
        public void updateSelection() {//Перерасчёт выбора по салу и перерисовка.
            sel.setStart(SoundManager.salo.from);
            sel.setEnd(SoundManager.salo.to);
            redrawSelection();
        }
        public void redrawSelection() {
            sel.rect.Width = ppf * sel.length;
            Elements.setPosAtCanvas(sel.rect, ppf * sel.min, 0);

            Elements.setPosAtCanvas(sel.nrect, sel.changingLeft?ppf * sel.min:ppf*sel.max+ppf, 0);
        }

        public void play() {
            Audio.play(Audio.getPiece(sel.min, sel.length));
        }

        private void mWheel(object sender, MouseWheelEventArgs e) {
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
                startX += e.Delta;
            } else {
                int sf = mToX(e.GetPosition(canv).X);
                ppf *= e.Delta > 0 ? 1.2 : 0.8;
                if (ppf < 0.1) ppf = 0.1;
                else if (ppf > 10) ppf = 10;
                startX = -sf * ppf+e.GetPosition(holder).X;
                updatePpf();
            }
            Elements.setPosAtCanvas(canv, startX, 0);
            //updatePpf();
        }

        private void kDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Left:
                case Key.A:
                    sel.affect(-1);
                    setSaloBySelection();
                    e.Handled = true;
                    break;
                case Key.Right:
                case Key.D:
                    sel.affect(1);
                    setSaloBySelection();
                    e.Handled = true;//Без этой фигни будут элементы переключаться от стрелочек.
                    break;
                case Key.E:
                    sel.changingLeft = !sel.changingLeft;
                    redrawSelection();
                    break;
                case Key.Space:
                    play();
                    break;
            }
        }

        void updatePpf() {
            double x = 0;
            foreach (Rectangle rect in disFrames) {
                rect.Width = ppf;
                Elements.setPosAtCanvas(rect, x, 12);
                x += ppf;
            }
            foreach (SoundAtLoc s in SoundManager.sounds) {
                Elements.setPosAtCanvas(s.tb, ppf * s.from, 0);
                s.rect.Width = ppf * (s.to - s.from + 1);
                Elements.setPosAtCanvas(s.rect, ppf * s.from, 10);
            }
            redrawSelection();
        }


    }
}
