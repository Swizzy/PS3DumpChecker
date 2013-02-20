namespace PS3DumpChecker
{
    using System.Drawing;
    using System.Windows.Forms;

    internal class ListBoxItem {
        
        private readonly string _name;
        private readonly int _id;

        public ListBoxItem(int id, string name) {
            _id = id;
            _name = name;
        }

        public override string ToString() {
            return _name;
            //return string.Format("[{0}] {1}", _id, _name);
        }

        public int Value {
            get { return _id; }
        }
    }

    class CustomListBox : ListBox
    {
        public CustomListBox() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if ((Items[e.Index] is ListBoxItem))
            {
                var item = Items[e.Index] as ListBoxItem;
                var color = Color.Green;
                if (!Common.PartList.ContainsKey(item.Value))
                    return;
                e.DrawBackground();
                if (!Common.PartList[item.Value].Result)
                    color = Color.Red;
                TextRenderer.DrawText(e.Graphics,
                                      Items[e.Index].ToString().Trim(),
                                      e.Font,
                                      e.Bounds, 
                                      color,
                                      TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
            else
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), e.Font, e.Bounds, e.ForeColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var itemRect = GetItemRectangle(i);
                if (!e.ClipRectangle.IntersectsWith(itemRect))
                    continue;
                DrawItemEventArgs diea;
                if (SelectedIndices.Contains(i))
                    diea = new DrawItemEventArgs(e.Graphics,
                                                 Font,
                                                 itemRect,
                                                 i,
                                                 DrawItemState.Selected);
                else
                    diea = new DrawItemEventArgs(e.Graphics,
                                                 Font,
                                                 itemRect,
                                                 i,
                                                 DrawItemState.None);
                OnDrawItem(diea);
            }
        }
    }
}
