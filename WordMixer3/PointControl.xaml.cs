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
    public partial class PointControl : UserControl {
        public PointControl() {
            InitializeComponent();
        }

        public Action<int> onChange;

        public int value=0;

        private void leftClick(object sender, RoutedEventArgs e) {
            value--;
            oc(value);
            valueBox.Text = value.ToString();
        }

        private void rightClick(object sender, RoutedEventArgs e) {
            value++;
            oc(value);
            valueBox.Text = value.ToString();
        }

        private void txtChanged(object sender, TextChangedEventArgs e) {
            if (valueBox.IsKeyboardFocused) {
                if(int.TryParse(valueBox.Text, out value))oc(value);
            }
        }

        public void setValue(int s) {
            value = s;
            oc(value);
            valueBox.Text = value.ToString();
        }

        void oc(int v) {
            if (onChange != null) onChange(v);
        }
    }
}
