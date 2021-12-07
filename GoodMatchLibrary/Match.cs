using System;

namespace GoodMatchLibrary
{
    public class Match
    {
        public string ans = "";
        private List<int> list = new List<int>();
        public string person1 = "";
        public string person2 = "";

        //Main function that will create frequency list of all letters and call get % function
        public string getMatch(string s1, string s2)
        {
            //Creating map with frequency of each letter
            Dictionary<char, int> map = new Dictionary<char, int>();
            person1 = s1.ToLower();
            person2 = s2.ToLower();
            string s = s1 + "matches" + s2;

            //looping over string that has both names in it and counting each oucurance of letters and stroing in map
            foreach (char ch in s.Replace(" ", string.Empty))
            {
                if (map.ContainsKey(ch))
                {
                    map[ch]++;
                }
                else
                {
                    map[ch] = 1;
                }
            }


            //looping over map and creating string of all ouccurances of digits
            foreach (var item in map.Keys)
            {


                //Look if digit ouccurs 10 or more times
                if (map[item] >= 10)
                {
                    int temp = map[item];
                    List<int> firstlist = new List<int>();
                    int count = map[item].ToString().Length;
                    for (int i = 0; i < count; i++)
                    {
                        firstlist.Add(temp % 10);
                        temp = temp / 10;
                    }
                    //reverses list as it's not in correct order to add to frequency string
                    firstlist.Reverse();

                    //adding the to list in correct order
                    foreach (var val in firstlist)
                        list.Add(val);

                }

                else
                {
                    list.Add(map[item]);
                }

            }

            //calling function to get the value to 2 digits
            while (list.Count > 2)
            {
                getPercentage();
            }

            foreach (var val in list)
                ans = ans + val;
            return ans;
        }


        //function that takes frequency and adds outside digits with each other
        private void getPercentage()
        {
            List<int> temp = new List<int>();
            int count = list.Count;

            //looping over list from start to half way and adds inner and outer digits
            for (int i = 0; i < count / 2; i++)
            {
                //adds first and last value
                int sum = list[i] + list[count - i - 1];

                //looks if sum is greater that 10 to add in list in correct order else it will be added as one value
                if (sum >= 10)
                {
                    temp.Add(1);
                    temp.Add(sum - 10);
                }
                else
                {
                    temp.Add(sum);
                }
            }

            //if odd number of digits, adds middle value to end of list
            if (count % 2 != 0)
            {

                temp.Add(list[count / 2]);
            }
            list = temp;

        }

    }
}
