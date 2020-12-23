
namespace KD.Model.Common
{
    public class ModelComboBox
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public ModelComboBox(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
