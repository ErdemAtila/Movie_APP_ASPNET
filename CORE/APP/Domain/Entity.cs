namespace CORE.APP.Domain
{
    public abstract class Entity // Entity e = new Entity(); e.Id = 5; e.Guid = "Leo";
    {
        //private int id; // field

        //public int GetId() // behavior
        //{
        //    return id;
        //}

        //public void SetId(int id)
        //{
        //    this.id = id;
        //}

        public int Id { get; set; } // property

        public string Guid { get; set; }
    }
}
