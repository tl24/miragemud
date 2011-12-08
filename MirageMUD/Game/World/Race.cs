using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;
using System.IO;

namespace Mirage.Game.World
{


    public class Race
    {
        public static void SaveRaces()
        {
            List<Race> races = new List<Race>();
            races.Add(new Race("Human", new BaseStats(15, 15, 15), true));
            races.Add(new Race("Ogre", new BaseStats(18, 13, 14), true));
            races.Add(new Race("Orc", new BaseStats(16, 16, 13), true));
            races.Add(new Race("Giant", new BaseStats(19, 12, 13), true));
            using (Stream s = new FileStream("races.jsx", FileMode.Create))
            {
                Serializer serializer = new Serializer(typeof(List<Race>));
                serializer.Serialize(races, s);
            }

        }
        private string _name;
        private BaseStats _defaultStats;
        private bool _isPlayerRace;

        public Race(string Name, BaseStats DefaultStats, bool IsPlayerRace)
        {
            this._name = Name;
            this._defaultStats = DefaultStats;
            this._isPlayerRace = IsPlayerRace;
        }

        [ConstructorParameter(0)]
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        [ConstructorParameter(1)]
        public Mirage.Game.World.BaseStats DefaultStats
        {
            get
            {
                return this._defaultStats;
            }
        }

        [ConstructorParameter(2)]
        public bool IsPlayerRace
        {
            get
            {
                return this._isPlayerRace;
            }
        }


    }

    public class BaseStats
    {
        private int _strength = 15;
        private int _dexterity = 15;
        private int _magic = 15;

        public BaseStats(int Strength, int Dexterity, int Magic)
        {
            this._strength = Strength;
            this._dexterity = Dexterity;
            this._magic = Magic;
        }

        [ConstructorParameter(0)]
        public int Strength
        {
            get
            {
                return this._strength;
            }
        }

        [ConstructorParameter(1)]
        public int Dexterity
        {
            get
            {
                return this._dexterity;
            }
        }

        [ConstructorParameter(2)]
        public int Magic
        {
            get
            {
                return this._magic;
            }
        }


    }
}
