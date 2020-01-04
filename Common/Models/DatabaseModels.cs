using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Question
    {
        public string id;
        public string question;
        public List<string> answers = new List<string>();
        public List<int> votes = new List<int>();      
    }    

    public class Result
    {
        public string question;
        public List<Vote> results;
    }      
    
    public class Vote
    {
        public int answer;
        public int count;
    }
}
