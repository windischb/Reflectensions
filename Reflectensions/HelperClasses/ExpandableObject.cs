using System;
using System.Collections.Generic;
using Reflectensions.ExtensionMethods;
using Reflectensions.Internal;

namespace Reflectensions.HelperClasses {

    public class ExpandableObject : ExpandableBaseObject {
        public ExpandableObject() : base() {

        }

        public ExpandableObject(object @object) : base(@object) {

        }

        public ExpandableObject(IDictionary<string, object> dictionary) {

            foreach (var keyValuePair in dictionary) {
                this[keyValuePair.Key] = keyValuePair.Value;
            }

        }

        public ExpandableObject(Dictionary<string, object> dictionary) {

            foreach (var keyValuePair in dictionary) {
                this[keyValuePair.Key] = keyValuePair.Value;
            }

        }


        public ExpandableObject(ExpandableObject expandableObject) {

            foreach (var keyValuePair in expandableObject.AsDictionary()) {
                this[keyValuePair.Key] = keyValuePair.Value;
            }

        }


        public static implicit operator ExpandableObject(Dictionary<string, object> dictionary) {

            return new ExpandableObject(dictionary);

        }
    }

}
