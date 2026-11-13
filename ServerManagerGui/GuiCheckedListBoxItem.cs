namespace blog.dachs.ServerManager
{
    internal class GuiCheckedListBoxItem
    {
        private String text;
        private object value;

        public GuiCheckedListBoxItem(String text, object value)
        {
            this.text = text;
            this.value = value;
        }

        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        public object Value
        { 
            get { return this.value; }
            set { this.value = value; }
        }

        public override String ToString()
        {
            return this.Text;
        }
    }
}
