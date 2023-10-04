namespace blog.dachs.ServerManager
{
    internal class ServerManagerGuiCheckedListBoxItem
    {
        private string text;
        private object value;

        public ServerManagerGuiCheckedListBoxItem(string text, object value)
        {
            this.text = text;
            this.value = value;
        }

        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        public object Value
        { 
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
