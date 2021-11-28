/**
 * Owned By: Chandan 
 * Created By: Chandan
 * Date Created: 25/11/2021
 * Date Modified: 25/11/2021
**/

using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;  
using Whiteboard;

namespace Testing.Whiteboard
{

    class ServerCheckPointHandlerTesting
    {
        private IServerCheckPointHandler _serverCheckPointHandler;

        // Instantiate random number generator.  
        private readonly Random _random = new Random();

        // Utility function to Generates a random number within a range.      
        private int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        //  Utility function to Generates a random string with a given size.    
        private string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        //Utitility function to generate random boardShapes
        private List<BoardShape> GenerateBoardShapes()
        {
            List<BoardShape> boardShapes = new();

            for (int i = 0; i < 5; i++)
            {
                boardShapes.Add(new(null,
                                   RandomNumber(0, 2),
                                   DateTime.Now,
                                   DateTime.Now.AddMinutes(i),
                                   RandomString(8),
                                   RandomString(8),
                                   Operation.CREATE));
            }
            return boardShapes;
        }

        //Utility function to compare objects of List<BoardShape> type
        private bool IsEqual(List<BoardShape> boardShapes1,List<BoardShape> boardShapes2)
        {
            if (boardShapes1.Count == boardShapes2.Count)
            {
                for(int i = 0; i < boardShapes1.Count; i++)
                {
                    if (boardShapes1[i].CreationTime != boardShapes2[i].CreationTime
                        || boardShapes1[i].LastModifiedTime != boardShapes2[i].LastModifiedTime
                        || boardShapes1[i].MainShapeDefiner != boardShapes2[i].MainShapeDefiner
                        || boardShapes1[i].ShapeOwnerId != boardShapes2[i].ShapeOwnerId
                        || boardShapes1[i].Uid != boardShapes2[i].Uid
                        || boardShapes1[i].UserLevel != boardShapes2[i].UserLevel
                        || boardShapes1[i].RecentOperation != boardShapes2[i].RecentOperation)
                    {
                        System.Diagnostics.Debug.WriteLine("something is wrong");

                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<int> _checkpointNumbers ;
        private List<Tuple<int, String, List<BoardShape>>> _checkpointSummary;

        [SetUp]
        public void SetUp()
        {
            // common Arrange
            _serverCheckPointHandler = new ServerCheckPointHandler();

            _checkpointSummary = new();
            _checkpointNumbers = new();
            for (int i = 1; i <= 4; i++)
            {
                _checkpointSummary.Add(new Tuple<int, string, List<BoardShape>>(i, RandomString(8), GenerateBoardShapes()));

            }
            foreach (var checkpoint in _checkpointSummary)
            {
                int current_CheckpointNumber = _serverCheckPointHandler.SaveCheckpoint(checkpoint.Item3, checkpoint.Item2);
                _checkpointNumbers.Add(current_CheckpointNumber);
            }

        }

        [Test]
        public void SaveCheckpoint_testing()
        {
            

            bool flag = true;
            for(int i = 1; i <= 4; i++)
            {
                var filePath = i.ToString()+".json";
                if (!File.Exists(filePath))
                {
                    flag = false;
                    break;
                }

                if (_checkpointNumbers[i-1] != i)
                {
                    flag = false;
                }

            }
            
            Assert.AreEqual(flag, true);

            
        }

        [Test]
        public void GetCheckPointNumber_returns_checkPointNumber()
        {
         
            int checkpointNumbers = _serverCheckPointHandler.GetCheckpointsNumber();
            Assert.AreEqual(4,checkpointNumbers);
        }

        [Test]
        public void FetchCheckpoint_Testing()
        {


            List<List<BoardShape>> fetched_boardShapes = new();
            for (int i = 1; i <= 4; i++)
            {
                fetched_boardShapes.Add(_serverCheckPointHandler.FetchCheckpoint(i));
            }

            bool flag = true;
            
            for(int i = 0; i < 4; i++)
            {
                if (!IsEqual(fetched_boardShapes[i],_checkpointSummary[i].Item3))
                {
                    flag = false;
                }
            }

            Assert.AreEqual(flag, true);

        }

        [Test]
        public void FetchCheckpoint_Fails()
        {

            var boardShape=_serverCheckPointHandler.FetchCheckpoint(5);
            Assert.IsNull(boardShape);

        }

        [Test]
        public void CheckPointSummary_Testing()
        {
            var CheckPointSummary = _serverCheckPointHandler.Summary();

            bool flag = true;
            if (CheckPointSummary.Count == _checkpointSummary.Count)
            {
                for(int i = 0; i < CheckPointSummary.Count; i++)
                {
                    if(CheckPointSummary[i].Item1!=_checkpointSummary[i].Item1 
                       || CheckPointSummary[i].Item2!=_checkpointSummary[i].Item2
                       || !IsEqual(CheckPointSummary[i].Item3, CheckPointSummary[i].Item3))
                    {
                        flag = false;
                    }
                }
            }
            Assert.IsTrue(flag);
        }


    }
}




