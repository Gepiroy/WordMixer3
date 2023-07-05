using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для MixPanel.xaml
    /// </summary>
    public partial class MixPanel : UserControl {
        List<MixSound> activeMixSounds = new List<MixSound>();
        Rectangle insertGlowing = new Rectangle {
            Fill = new SolidColorBrush(Color.FromArgb(125, 0, 120, 255)),
            Height = 54,
            Width = 2
        };
        public MixPanel() {
            InitializeComponent();
            genMix("сатан'истыбл'я");
        }

        public void genMix(string st) {
            foreach (MixSound s in activeMixSounds.ToArray()) {
                remSound(s);
            }
            foreach(MixSound s in new MixSound(st).findEverything()) {
                addSound(s);
            }
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            draw();
        }
        private static double ppf = 6;

        public void draw() {
            if (ActualWidth <= 0 || ActualHeight <= 0 || double.IsNaN(ActualHeight) || double.IsNaN(ActualWidth)) return;
            double x = 0;
            for (int i = 0; i < activeMixSounds.Count; i++) {
                x += ppf * activeMixSounds[i].gap;
                Rectangle rect = activeMixSounds[i].rect;
                rect.Width = ppf * activeMixSounds[i].length-2;//-2 border
                Elements.setPosAtCanvas(activeMixSounds[i].comp, x, 12);
                x += ppf* activeMixSounds[i].length;
            }
        }
        MixSound sAtX(int x) {
            int tx = 0;
            foreach (MixSound s in activeMixSounds) {
                if (x>=tx&&x < tx+s.fullLength) {
                    return s;
                }
                tx += s.fullLength;
            }
            return null;
        }
        int mToX(double mp) {
            return (int)(mp / ppf);
        }
        void remSound(MixSound s) {
            activeMixSounds.Remove(s);
            canv.Children.Remove(s.comp);
            InvalidateVisual();
        }
        public void addSound(MixSound s) {
            activeMixSounds.Add(s);
            canv.Children.Add(s.comp);
            InvalidateVisual();
        }
        void insertSound(MixSound s, int index) {
            activeMixSounds.Insert(index, s);
            canv.Children.Add(s.comp);
            InvalidateVisual();
        }
        int startX(MixSound s) {
            int x = 0;
            foreach (MixSound ts in activeMixSounds) {
                if (ts == s) break;
                x += ts.fullLength;
            }
            return x;
        }

        MixSound pickedUp = null;
        int clickedX;
        bool holding = false;
        MixSound choosen = null;
        void setChoosen(MixSound s) {
            if (choosen != null) choosen.border.BorderBrush = Brushes.Black;
            choosen = s;
            if(choosen!=null) choosen.border.BorderBrush = Brushes.Yellow;
        }
        private void mMove(object sender, MouseEventArgs e) {
            int x = mToX(e.GetPosition(this).X);
            if (holding&&pickedUp==null) {
                pickedUp = sAtX(clickedX);
                if (pickedUp != null) {
                    remSound(pickedUp);
                    canv.Children.Add(insertGlowing);
                }
            }
            if (pickedUp!=null) {
                MixSound at = sAtX(x);
                Elements.setPosAtCanvas(insertGlowing, startX(at)*ppf - 1, 20);
            }
        }
        private void mDown(object sender, MouseButtonEventArgs e) {
            clickedX = mToX(e.GetPosition(this).X);
            holding = true;
        }

        private void mUp(object sender, MouseButtonEventArgs e) {
            holding = false;
            int x = mToX(e.GetPosition(this).X);
            if (pickedUp!=null) {
                //Place it somewhere
                MixSound at = sAtX(x);
                if (at == null) addSound(pickedUp);
                else insertSound(pickedUp,activeMixSounds.IndexOf(at));
                pickedUp = null;
                canv.Children.Remove(insertGlowing);
            } else {
                setChoosen(sAtX(x));
                
            }
        }

        private void kDown(object sender, KeyEventArgs e) {
            //if (e.Key == Key.Delete&&choosen!=null) {
                //remSound(choosen);
                //setChoosen(null);
            //}
        }

        public byte[] getBytes() {
            using (MemoryStream ms = new MemoryStream()) {
                foreach (MixSound m in activeMixSounds) {
                    if (m.gap != 0) write(ms, Audio.genVoid(m.gap));
                    if (m.source == null) write(ms, Audio.genVoid(1));
                    else write(ms, Audio.getPiece(m.from, m.length));
                }
                return ms.ToArray();
            }
        }

        void write(MemoryStream ms, byte[] bytes) {
            ms.Write(bytes, 0, bytes.Length);
        }

        /*
* Управление. В мельчайших деталях.
* Нужны функции: переноса, увеличения-уменьшения, увел-уменьш гэпов.
* Короче, справа и слева на границах объектов в верхней части есть возможность растягивать/ужимать отрезки.
* А в нижней части левой границы можно потянуть и изменить гэп.
* 
* Удаление. Нужно добавить возможность выбирать объект, ну и прослушку клавы под delete.
* После этих двух обнов полноценное управление дорожкой готово.
* 
* Проигрывание.
* Нужна простейшая функция проигрывания итогового вордмикса, пока что пофиг на выделения частей. В будущем понадобятся клавобинды.
* 
* Рендер. Тупо то же самое проигрывание, обнова на 10 минут.
* 
* Ручное добавление.
* То что снизу выделил - кнопку долбанул - добавился новый миксаунд. Обнова на 15 минут.
* 
* Ввод под автоген.
* Просто добавляем на базовую панель (не сюда) поле и кнопку. Она чистит текущий микс и генит новый по уже готовому алгоритму.
*/

    }
}
