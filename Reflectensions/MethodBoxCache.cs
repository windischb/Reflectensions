using System;
using System.Collections.Generic;
using System.Linq;

namespace doob.Reflectensions {
    internal class MethodBoxCache<TBox> where TBox : IMethodBox {
        private readonly Dictionary<string, TBox> _methodInfoBoxes = new Dictionary<string, TBox>();

        private readonly object _dictionaryLock = new object();

        internal TBox Get(MethodSearch search) {

            lock (_dictionaryLock) {
                var sig = search.ToString();
                if (_methodInfoBoxes.ContainsKey(sig))
                    return _methodInfoBoxes[sig];

                return default(TBox);
            }
        }

        internal TBox Add(MethodBoxBuilderResult<TBox> builderResult) {

            lock (_dictionaryLock) {
                var sig = builderResult.CustomSearch?.ToString() ?? MethodSearch.FromMethodInfo(builderResult.MethodInfoBox.MethodInfo).ToString();
                if (_methodInfoBoxes.ContainsKey(sig)) {
                    throw new Exception("A Mehod with same Key already exists!");
                } else {
                    _methodInfoBoxes.Add(sig, builderResult.MethodInfoBox);
                }


                return _methodInfoBoxes[sig];
            }
        }


        internal TBox Add(MethodSearch search, TBox box) {

            lock (_dictionaryLock) {
                var sig = search.ToString();
                if (_methodInfoBoxes.ContainsKey(sig)) {
                        throw new Exception("A Mehod with same Key already exists!");
                    } else {
                        _methodInfoBoxes.Add(sig, box);
                    }
                

                return _methodInfoBoxes[sig];
            }
        }
        
        internal TBox GetOrAdd(MethodSearch search, Func<TBox> box) {
            lock (_dictionaryLock) {
                var sig = search.ToString();
                if (_methodInfoBoxes.ContainsKey(sig)) {
                    return _methodInfoBoxes[sig];
                }

                _methodInfoBoxes.Add(sig, box.Invoke());
                return _methodInfoBoxes[sig];
            }
        }

    }
}
