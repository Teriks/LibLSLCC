#region


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibraryDataScrapingTools.LibraryDataScrapers.FirestormLibraryDataDom;
using LibraryDataScrapingTools.ScraperInterfaces;


#endregion

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class FirestormLibraryData : ILibraryData
    {
        private readonly LSLEventSignatureRegex _eventSig = new LSLEventSignatureRegex("", ";*");
        private readonly LSLFunctionSignatureRegex _functionSig = new LSLFunctionSignatureRegex("", ";*");
        private readonly ScriptLibrary _scriptLibrary;

        public FirestormLibraryData(XmlReader reader)
        {
            _scriptLibrary = ScriptLibrary.Read(reader);
        }

        public FirestormLibraryData(ScriptLibrary library)
        {
            _scriptLibrary = library;
        }

        public bool LSLFunctionExist(string name)
        {
            return _scriptLibrary.Functions.Contains(name);
        }

        public bool LSLConstantExist(string name)
        {
            return _scriptLibrary.Keywords.Contains(name) && _scriptLibrary.Keywords.Get(name) is IConstant;
        }

        public bool LSLEventExist(string name)
        {
            return _scriptLibrary.Keywords.Contains(name) && _scriptLibrary.Keywords.Get(name) is Event;
        }

        public IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            var sigs = new HashSet<LSLLibraryFunctionSignature>();
            foreach (var overload in _scriptLibrary.Functions.Get(name))
            {
                var matches = _functionSig.Regex.Matches(overload.Desc);


                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        var sig = LSLLibraryFunctionSignature.Parse(match.ToString());

                        sig.DocumentationString = _functionSig.Regex.Replace(overload.Desc, "");

                        if (!sigs.Contains(sig))
                        {
                            Log.WriteLine(
                                "FirestormLibraryData script_library name={0}: function {1} signature {2} found in doc string",
                                _scriptLibrary.Name, overload.Name, sig.SignatureString);
                            sigs.Add(sig);
                        }
                        else if (matches.Count == 1)
                        {
                            Log.WriteLine(
                                "FirestormLibraryData script_library name={0}: function {1} has a duplicate overload caused by multiple xml node definitions",
                                _scriptLibrary.Name, sig.Name);
                        }
                        else
                        {
                            Log.WriteLine(
                                "FirestormLibraryData script_library name={0}: function {1} has a duplicate overload caused by the documentation signature",
                                _scriptLibrary.Name, overload.Name);
                        }
                    }
                    else
                    {
                        Log.WriteLine(
                            "FirestormLibraryData script_library name={0}: function {1} has no signature in doc string",
                            _scriptLibrary.Name, overload.Name);
                    }
                }
            }

            return sigs.ToList();
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            var c = _scriptLibrary.Keywords.Get(name);

            if (c is IntegerConstant)
            {
                return new LSLLibraryConstantSignature(LSLType.Integer, c.Name) {DocumentationString = c.Desc};
            }
            if (c is FloatConstant)
            {
                return new LSLLibraryConstantSignature(LSLType.Float, c.Name) {DocumentationString = c.Desc};
            }
            if (c is StringConstant)
            {
                return new LSLLibraryConstantSignature(LSLType.String, c.Name) {DocumentationString = c.Desc};
            }
            var cmpnd = c as CompoundConstant;
            if (cmpnd != null)
            {
                var components = cmpnd.Desc.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Length;
                return new LSLLibraryConstantSignature(components == 3 ? LSLType.Vector : LSLType.Rotation, c.Name)
                {
                    DocumentationString = c.Desc
                };
            }

            return null;
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            var ev = _scriptLibrary.Keywords.Get(name) as Event;

            if (ev != null)
            {
                var match = _eventSig.Regex.Match(ev.Desc);
                if (match.Success)
                {
                    var sig = LSLLibraryEventSignature.Parse(match.ToString());

                    sig.DocumentationString = _eventSig.Regex.Replace(ev.Desc, "");

                    Log.Write(
                        "FirestormLibraryData script_library name={0}: event {1} signature {2} found in docstring",
                        _scriptLibrary.Name,
                        ev.Name, sig.SignatureString);

                    return sig;
                }
                Log.Write("FirestormLibraryData script_library name={0}: event {1} has no signature in doc string",
                    _scriptLibrary.Name,
                    ev.Name);
                return null;
            }
            return null;
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            foreach (var fs in _scriptLibrary.Functions.OverloadGroups)
            {
                var sigs = new HashSet<LSLLibraryFunctionSignature>();
                foreach (var f in fs)
                {
                    var matches = _functionSig.Regex.Matches(f.Desc);


                    if (matches.Count == 0)
                    {
                        Log.WriteLine(
                            "FirestormLibraryData script_library name={0}: function {1} has no signature in doc string",
                            _scriptLibrary.Name, f.Name);
                        continue;
                    }

                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            var sig = LSLLibraryFunctionSignature.Parse(match.ToString());
                            sig.DocumentationString = _functionSig.Regex.Replace(f.Desc, "");

                            if (!sigs.Contains(sig))
                            {
                                sigs.Add(sig);
                            }
                            else if (matches.Count == 1)
                            {
                                Log.WriteLine(
                                    "FirestormLibraryData script_library name={0}: function {1} has a duplicate overload caused by multiple xml node definitions",
                                    _scriptLibrary.Name, sig.Name);
                            }
                            else
                            {
                                Log.WriteLine(
                                    "FirestormLibraryData script_library name={0}: function {1} has a duplicate overload caused by the documentation signature",
                                    _scriptLibrary.Name, sig.Name);
                            }
                        }
                    }
                }
                yield return sigs.ToList();
            }
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            var sigs = new HashSet<LSLLibraryFunctionSignature>();
            foreach (var f in _scriptLibrary.Functions)
            {
                var matches = _functionSig.Regex.Matches(f.Desc);

                if (matches.Count == 0)
                {
                    Log.WriteLine(
                        "FirestormLibraryData script_library name={0}: function {1} has no signature in doc string",
                        _scriptLibrary.Name, f.Name);
                    yield break;
                }

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        var sig = LSLLibraryFunctionSignature.Parse(match.ToString());

                        sig.DocumentationString = _functionSig.Regex.Replace(f.Desc, "");

                        if (!sigs.Contains(sig))
                        {
                            sigs.Add(sig);

                            yield return sig;
                        }
                    }
                }
            }
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            foreach (var c in _scriptLibrary.Keywords)
            {
                if (c is IntegerConstant)
                {
                    yield return new LSLLibraryConstantSignature(LSLType.Integer, c.Name) {DocumentationString = c.Desc}
                        ;
                }
                if (c is FloatConstant)
                {
                    yield return new LSLLibraryConstantSignature(LSLType.Float, c.Name) {DocumentationString = c.Desc};
                }
                if (c is StringConstant)
                {
                    yield return new LSLLibraryConstantSignature(LSLType.String, c.Name) {DocumentationString = c.Desc};
                }
                var cmpnd = c as CompoundConstant;
                if (cmpnd != null)
                {
                    var components = cmpnd.Desc.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Length;
                    yield return
                        new LSLLibraryConstantSignature(components == 3 ? LSLType.Vector : LSLType.Rotation, c.Name)
                        {
                            DocumentationString = c.Desc
                        };
                }
            }
        }

        public IEnumerable<LSLLibraryEventSignature> LSLEvents()
        {
            foreach (var ev in _scriptLibrary.Keywords.Where(x => x is Event))
            {
                var match = _eventSig.Regex.Match(ev.Desc);
                if (match.Success)
                {
                    var sig = LSLLibraryEventSignature.Parse(match.ToString());

                    sig.DocumentationString = _eventSig.Regex.Replace(ev.Desc, "");

                    yield return sig;
                }
                else
                {
                    Log.Write("FirestormLibraryData script_library name={0}: event {1} has no signature in doc string",
                        _scriptLibrary.Name,
                        ev.Name);
                }
            }
        }
    }

    namespace FirestormLibraryDataDom
    {
        [XmlInclude(typeof (DataType))]
        [XmlInclude(typeof (StringConstant))]
        [XmlInclude(typeof (IntegerConstant))]
        [XmlInclude(typeof (FloatConstant))]
        [XmlInclude(typeof (CompoundConstant))]
        [XmlInclude(typeof (FlowControlKeyword))]
        [XmlInclude(typeof (FlowControlLabel))]
        [XmlInclude(typeof (Section))]
        [XmlInclude(typeof (Comment))]
        [XmlInclude(typeof (BlockComment))]
        [XmlInclude(typeof (Event))]
        [XmlRoot(ElementName = "lsl_keyword")]
        public class Keyword
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "desc")]
            public string Desc { get; set; }
        }

        [XmlRoot(ElementName = "function")]
        public class Function : IEquatable<Function>
        {
            [XmlAttribute(AttributeName = "sleep")]
            public string Sleep { get; set; }

            [XmlAttribute(AttributeName = "energy")]
            public string Energy { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "desc")]
            public string Desc { get; set; }

            public bool Equals(Function other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Sleep, other.Sleep) && string.Equals(Energy, other.Energy) &&
                       string.Equals(Name, other.Name) && string.Equals(Desc, other.Desc);
            }

            public override string ToString()
            {
                return Name + ": " + Desc;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (Sleep != null ? Sleep.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Energy != null ? Energy.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Desc != null ? Desc.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public static bool operator ==(Function left, Function right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Function left, Function right)
            {
                return !Equals(left, right);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Function) obj);
            }
        }

        public interface IConstant
        {
            LSLLibraryConstantSignature GetSignature(IReadOnlyList<string> subsets);
        }

        [XmlRoot(ElementName = "data_type")]
        public class DataType : Keyword
        {
        }

        [XmlRoot(ElementName = "section")]
        public class Section : Keyword
        {
        }

        [XmlRoot(ElementName = "integer_constant")]
        public class IntegerConstant : Keyword, IConstant
        {
            public LSLLibraryConstantSignature GetSignature(IReadOnlyList<string> subsets)
            {
                var f = new LSLLibraryConstantSignature(LSLType.Integer, Name) {DocumentationString = Desc};
                f.SetSubsets(subsets);
                return f;
            }
        }

        [XmlRoot(ElementName = "float_constant")]
        public class FloatConstant : Keyword, IConstant
        {
            public LSLLibraryConstantSignature GetSignature(IReadOnlyList<string> subsets)
            {
                var f = new LSLLibraryConstantSignature(LSLType.Float, Name) {DocumentationString = Desc};
                f.SetSubsets(subsets);
                return f;
            }
        }

        [XmlRoot(ElementName = "string_constant")]
        public class StringConstant : Keyword, IConstant
        {
            public LSLLibraryConstantSignature GetSignature(IReadOnlyList<string> subsets)
            {
                var f = new LSLLibraryConstantSignature(LSLType.String, Name) {DocumentationString = Desc};
                f.SetSubsets(subsets);
                return f;
            }
        }

        [XmlRoot(ElementName = "compound_constant")]
        public class CompoundConstant : Keyword, IConstant
        {
            public LSLLibraryConstantSignature GetSignature(IReadOnlyList<string> subsets)
            {
                string[] s = Desc.Split(',');
                LSLType t = s.Length == 3 ? LSLType.Vector : LSLType.Rotation;
                var f = new LSLLibraryConstantSignature(t, Name) {DocumentationString = Desc};
                f.SetSubsets(subsets);
                return f;
            }
        }

        [XmlRoot(ElementName = "flow_control_keyword")]
        public class FlowControlKeyword : Keyword
        {
        }

        [XmlRoot(ElementName = "flow_control_label")]
        public class FlowControlLabel : Keyword
        {
        }

        [XmlRoot(ElementName = "comment")]
        public class Comment : Keyword
        {
        }

        [XmlRoot(ElementName = "block_comment")]
        public class BlockComment : Keyword
        {
        }

        [XmlRoot(ElementName = "event")]
        public class Event : Keyword
        {
        }

        public class KeywordCollection : ICollection<Keyword>
        {
            private readonly Dictionary<string, Keyword> _items = new Dictionary<string, Keyword>();

            public IEnumerator<Keyword> GetEnumerator()
            {
                return _items.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _items).GetEnumerator();
            }

            public void Add(Keyword item)
            {
                try
                {
                    _items.Add(item.Name, item);
                }
                catch (ArgumentException)
                {
                    Log.WriteLine("KeywordCollection: Duplicate keyword " + item.Name + " in firestorm script library");
                }
            }

            public void Clear()
            {
                _items.Clear();
            }

            public bool Contains(Keyword item)
            {
                return _items.ContainsKey(item.Name);
            }

            public void CopyTo(Keyword[] array, int arrayIndex)
            {
                _items.Values.CopyTo(array, arrayIndex);
            }

            public bool Remove(Keyword item)
            {
                return _items.Remove(item.Name);
            }

            public int Count
            {
                get { return _items.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public Keyword Get(string name)
            {
                return _items[name];
            }

            public bool Contains(string name)
            {
                return _items.ContainsKey(name);
            }
        }


        public class FunctionCollection : ICollection<Function>
        {
            private readonly Dictionary<string, List<Function>> _items = new Dictionary<string, List<Function>>();

            [XmlIgnore]
            public IEnumerable<IList<Function>> OverloadGroups
            {
                get { return _items.Values; }
            }

            public IEnumerator<Function> GetEnumerator()
            {
                return _items.Values.SelectMany(y => y).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _items.Values.SelectMany(y => y)).GetEnumerator();
            }

            public void Add(Function item)
            {
                if (Contains(item))
                {
                    Log.WriteLine("FunctionCollection: Duplicate function " + item.Name + " in firestorm script library");
                    return;
                }

                if (_items.ContainsKey(item.Name))
                {
                    _items[item.Name].Add(item);
                }
                else
                {
                    _items.Add(item.Name, new List<Function> {item});
                }
            }

            public void Clear()
            {
                _items.Clear();
            }

            public bool Contains(Function item)
            {
                if (_items.ContainsKey(item.Name))
                {
                    return _items[item.Name].Any(x => x == item);
                }

                return false;
            }

            public void CopyTo(Function[] array, int arrayIndex)
            {
                _items.Values.SelectMany(y => y).ToArray().CopyTo(array, arrayIndex);
            }

            public bool Remove(Function item)
            {
                return _items.Remove(item.Name);
            }

            public int Count
            {
                get { return _items.Values.SelectMany(x => x).Count(); }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IList<Function> Get(string name)
            {
                return _items[name];
            }

            public bool Contains(string name)
            {
                return _items.ContainsKey(name);
            }
        }

        [XmlRoot(ElementName = "script_library")]
        public class ScriptLibrary
        {
            private FunctionCollection _functions = new FunctionCollection();
            private KeywordCollection _keywords = new KeywordCollection();

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlArray("keywords")]
            [XmlArrayItem("data_type)", Type = typeof (DataType))]
            [XmlArrayItem("string_constant", Type = typeof (StringConstant))]
            [XmlArrayItem("integer_constant", Type = typeof (IntegerConstant))]
            [XmlArrayItem("float_constant", Type = typeof (FloatConstant))]
            [XmlArrayItem("compound_constant", Type = typeof (CompoundConstant))]
            [XmlArrayItem("flow_control_keyword", Type = typeof (FlowControlKeyword))]
            [XmlArrayItem("flow_control_label", Type = typeof (FlowControlLabel))]
            [XmlArrayItem("section", Type = typeof (Section))]
            [XmlArrayItem("comment", Type = typeof (Comment))]
            [XmlArrayItem("block_comment", Type = typeof (BlockComment))]
            [XmlArrayItem("event", Type = typeof (Event))]
            public KeywordCollection Keywords
            {
                get { return _keywords; }
                set { _keywords = value; }
            }

            [XmlArray(ElementName = "functions")]
            [XmlArrayItem(ElementName = "function")]
            public FunctionCollection Functions
            {
                get { return _functions; }
                set { _functions = value; }
            }

            public static ScriptLibrary Read(XmlReader reader)
            {
                var x = new XmlSerializer(typeof (ScriptLibrary));
                return (ScriptLibrary) x.Deserialize(reader);
            }
        }
    }
}