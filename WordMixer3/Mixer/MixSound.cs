using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WordMixer3 {
    public class MixSound {
        string targeting;
        public SoundAtLoc source { get; private set; }
        List<SoundAtLoc> ban = new List<SoundAtLoc> ();
        public int gap=0, from, to;
        public int length { get { return to - from + 1; } }
        public int fullLength { get { return to - from + 1+gap; } }
        MixSound parent;
        List<MixSound> Children = new List<MixSound>();

        public StackPanel comp;
        public Rectangle rect;
        public Border border;

        public int splittersInside;

        public MixSound(string targeting) {
            this.targeting = targeting;
            init();
        }

        public void setSource(SoundAtLoc sal) {
            source = sal;
            from = sal.from;
            to = sal.to;
        }

        MixSound(MixSound parent, string targeting, SoundAtLoc source) {
            this.parent = parent;
            this.targeting = targeting;
            setSource(source);
            init();
        }

        public MixSound(string targeting, SoundAtLoc source) {
            this.targeting = targeting;
            setSource(source);
            init();
        }

        void init() {
            comp = new StackPanel();
            comp.ContextMenu = new ContextMenu();
            MenuItem selectOriginalItem = new MenuItem {Header = "Выбрать на дорожке" };
            selectOriginalItem.Click += (s, e) => {
                MainWindow.instance.bowl.select(from, to);
                SoundManager.setSalo(source);
            };
            comp.ContextMenu.Items.Add(selectOriginalItem);
            genComp();
        }

        public List<MixSound> findEverything() {
            string[] splitted = splitTarget();
            splittersInside = splitted.Length;
            MixSound[] reret = new MixSound[splitted.Length];
            bool[] dones = new bool[splitted.Length];
            bool allDone = false;
            for (int l=splitted.Length;l>0;l--) {
                for (int f = 0; f <= splitted.Length-l; f++) {
                    string search = "";
                    for (int i = 0; i < l; i++) {
                        search += splitted[i + f];
                    }
                    SoundAtLoc sal = searchEqual(search);
                    if (sal != null) {
                        MixSound m = new MixSound(this, search, sal);
                        m.splittersInside = l;
                        if (!isCross(reret, m, f)) {
                            reret[f] = m;
                            for (int i = 0; i < l; i++) {
                                dones[i + f] = true;
                            }
                        }
                    }
                    allDone = true;
                    foreach (bool b in dones) if (!b) { allDone = false; break; }
                    if (allDone) break;
                }
                if (allDone) break;
            }
            if (!allDone) {
                /*
                 * Так. Если появились какие-то пробелы, например из "повесица" смогли найти "о" и "с", должна построиться моделька, где указано, каких звуков не нашли.
                 * То есть создаём пустые кадры под каждый отдельный звук, не фразу, а после - добавим возможность их удалять при ручной замене.
                 */
                for (int i=0;i<splitted.Length;i++) {
                    if (reret[i] == null) {
                        reret[i] = new MixSound(splitted[i]);
                    } else {
                        i += reret[i].splittersInside - 1;
                    }
                }
            }
            List<MixSound> ret = new List<MixSound>();
            foreach (MixSound m in reret) {
                if (m != null) ret.Add(m);
            }
            return ret;//ХЭЙ! А может, он вообще все-все-все варианты разом подбирать будет, чтобы мы потом чистили от ненужного?! Нет, это лишний труд... Но в разработке так лучше.
        }
        bool isCross(MixSound[] reret, MixSound s, int f) {
            int[] bans = new int[s.splittersInside];
            for (int i=0;i<bans.Length;i++) {
                bans[i] = f + i;
            }
            for (int i=0;i<reret.Length;i++) {
                if (reret[i] != null) {
                    for (int di = 0; di < reret[i].splittersInside; di++) {
                        foreach (int b in bans) if (i + di == b) return true;
                    }
                }
            }
            return false;
        }
        static SolidColorBrush transB = new SolidColorBrush(Color.FromArgb(100,255,255,255));
        static SolidColorBrush filledB = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public void genComp() {
            comp.Children.Clear();
            comp.Children.Add(new TextBlock { Text=targeting});
            border = new Border {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            rect=new Rectangle {
                Fill = source == null ? transB : filledB,
                Height = 50
        };
            border.Child = rect;
            comp.Children.Add(border);
        }

        SoundAtLoc searchEqual(string st) {//"п'е'"
            foreach (SoundAtLoc sal in SoundManager.sounds) {
                if (!ban.Contains(sal) && sal.sound.Equals(st)) return sal;
            }
            return null;
        }

        string[] splitTarget() {
            List<string> ret = new List<string>();
            for (int i=0;i<targeting.Length;i++) {
                if (targeting[i] == '\'') ret[ret.Count - 1] += targeting[i];
                else ret.Add(targeting[i]+"");
            }
            return ret.ToArray();
        }
    }
}
