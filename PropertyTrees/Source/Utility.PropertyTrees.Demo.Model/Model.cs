using System.Collections.ObjectModel;
using System.ComponentModel;
using Utility.Helpers.Ex;

namespace Utility.PropertyTrees.Demo.Model
{
    public class GameModel
    {
        //public Guid Guid => Guid.Parse("1edf3c6d-1424-4771-8672-0b77d7c44342");

        //public Game CurrentGame { get; set; }
        //public Game PreviousGame { get; set; }

        //public Animation Animation { get; set; }
        //public Game StartGame { get; set; }
        //public Game EndGame { get; set; }

        //public PrizeWheelUpdate PrizeWheelUpdate { get; set; }
       
        public ScreenSaverModel ScreenSaver { get; set; } = new();
        public PrizeWheelModel PrizeWheel { get; set; } = new(); 
        public LeaderboardModel Leaderboard { get; set; } = new();
    }


    public class ScreenSaverModel: JsonModel
    {
        public ScreenSaver ScreenSaver { get; set; }

        public void Send()
        {
            JSON = JsonHelper.Serialize(ScreenSaver);
        }
    }   
    
    public class PrizeWheelModel: JsonModel
    {
        public PrizeWheel PrizeWheel { get; set; }

        public void Send()
        {
            JSON = JsonHelper.Serialize(PrizeWheel);
        }
    }  
    
    public class LeaderboardModel: JsonModel
    {
        public Leaderboard Leaderboard { get; set; }

        public void Send()
        {
            JSON = JsonHelper.Serialize(Leaderboard);
        }
    }

    public class JsonModel: INotifyPropertyChanged
    {
        private string jSON;
        public string JSON
        {
            get => jSON; set
            {
                jSON = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JSON)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    //public class Game
    //{
    //    public string Name { get; set; }

    //    public List<Hole> Holes { get; set; }
    //}

    //public class Hole
    //{
    //    public string Course { get; set; }
    //    public string HoleNumber { get; set; }
    //    public string HoleName { get; set; }
    //    public int Points { get; set; }
    //    public bool Supertube { get; set; }
    //    public bool Hazard { get; set; }
    //    public bool HoleInOne { get; set; }
    //}

    //public class Animation
    //{
    //    public bool SuperTube { get; set; }
    //    public bool HoleInOne { get; set; }
    //    public bool Hazard { get; set; }
    //    public PointCalculator PointCalculator { get; set; }
    //    public bool ShotTaken { get; set; }
    //}

    //public class PointCalculator
    //{
    //    public int Points { get; set; }
    //    public int Sequence { get; set; }
    //}

    public enum PrizeWheelState
    {
        Running, Stopping
    }

    public class PrizeWheel
    {
        public PrizeWheelState State { get; set; }
        public PrizeWheelRunning Running { get; set; }

        public PrizeWheelStopping Stopping { get; set; }

    }

    public class PrizeWheelRunning 
    {
        public int Speed { get; set; }
    }

    public class PrizeWheelStopping 
    {
        public int ItemNumber { get; set; }
        public int Time { get; set; }
    }

    //public class PrizeWheel
    //{
    //    public int SegmentSequence { get; set; }
    //    public string Name { get; set; }
    //    public int ColourScheme { get; set; }
    //    public bool PrintTicket { get; set; }
    //    public int Size { get; set; }
    //    public bool AllowWin { get; set; }
    //}

    public class Leaderboard
    {
        public ObservableCollection<Leader> Leaders { get; set; } = new() { new Leader { Name = "John", Score = 1000 } };
    }

    public class Leader
    {
        public string Name { get; set; }    

        public int Score { get; set; }
    }

    public class ScreenSaver
    {
        public string FileName { get; set; }
    }
}