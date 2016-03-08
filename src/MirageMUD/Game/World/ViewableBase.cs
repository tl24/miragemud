
namespace Mirage.Game.World
{
    public abstract class ViewableBase : BaseData, IViewable
    {
        [Editor(Priority = 3)]
        public string Name { get; set; }

        [Editor(Priority = 4)]
        public string ShortDescription { get; set; }

        [Editor(Priority = 5, EditorType = "Multiline")]
        public string LongDescription { get; set; }

        public override void CopyTo(BaseData other)
        {
            base.CopyTo(other);
            ViewableBase otherViewable = other as ViewableBase;
            if (otherViewable != null)
            {
                otherViewable.Name = this.Name;
                otherViewable.ShortDescription = this.ShortDescription;
                otherViewable.LongDescription = this.LongDescription;
            }
        }
    }
}
