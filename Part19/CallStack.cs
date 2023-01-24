using System;
using System.Collections.Generic;

namespace SPI
{
    enum ARType {
        PROGRAM,
        PROCEDURE
    };

    class ActivationRecord
    {
        public string               name;
        public ARType               type;
        int                         nesting_level;
        Dictionary<string,dynamic>  members;

        public ActivationRecord(string name, ARType type, int nesting_level)
        {
            this.name = name;
            this.type = type;
            this.nesting_level = nesting_level;
            members = new Dictionary<string, dynamic>();
        }

        public void SetItem(string key, dynamic value)
        {
            if(members.ContainsKey(key))
                members[key] = value;
            else
                members.Add(key, value);
        }

        public dynamic? GetItem(string key)
        {
            dynamic? value = null;
            if(members.ContainsKey(key))
                value = members[key];
            return value;
        }

        public string Str()
        {
            string s = $"{nesting_level}: {type.ToString()} {name}";
            foreach(string key in members.Keys) {
                s += $"\n    {key}: {members[key].ToString()}";
            }
            return s;
        }
    }

    class CallStack
    {
        public Stack<ActivationRecord> records;

        public CallStack()
        {
            records = new Stack<ActivationRecord>();
        }

        public void Push(ActivationRecord ar)
        {
            records.Push(ar);
        }

        public ActivationRecord Pop()
        {
            return records.Pop();
        }

        public ActivationRecord Peek()
        {
            return records.Peek();
        }

        public string Str()
        {
            string s = "CALL STACK\n";
            foreach(ActivationRecord item in records.Reverse()) {
                s += item.Str() + "\n";
            }
            return s;
        }
    }
}