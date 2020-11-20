using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        public int ID
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "p1")]
        public Vector2D p1
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "p2")]
        public Vector2D p2
        {
            get; private set;
        }

        public Wall()
        {

        }

        public Wall(int ID, Vector2D topLeft, Vector2D bottomRight)
        {
            this.ID = ID;
            this.p1 = topLeft;
            this.p2 = bottomRight;
        }
    }
}
