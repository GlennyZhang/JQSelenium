using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace JQSelenium
{
    /// <summary>
    /// It contains all the jQueryTags generated by the JQueryFactory during a Query.
    /// </summary>
    public class JQuerySelector : IEnumerable<JQueryTag>
    {
        #region Class Members
        /// <summary>
        /// Contains all the JQueryTags generated with the provided selector of the JQuerySelector
        /// </summary>
        private List<JQueryTag> _subset;

        /// <summary>
        /// The selector provided by the JQueryFactory.
        /// </summary>
        private string _selector;

        /// <summary>
        /// An index used to iterate over all of the JQueryTags manually.
        /// </summary>
        private int _iterator;

        /// <summary>
        /// Used to execute javaScript code.
        /// </summary>
        private readonly IJavaScriptExecutor _js;
        #endregion Class Members

        #region Constructors
        /// <summary>
        /// Initializes a new JQuerySelector
        /// </summary>
        public JQuerySelector()
        {
            this._subset = new List<JQueryTag>();
            this._selector = "";
            this._iterator = 0;
            this._js = null;
        }

        /// <summary>
        /// Initializes a new JQuerySelector
        /// </summary>
        /// <param name="jqt">Used to create a new JQuerySelector of a single JQueryTag</param>
        public JQuerySelector(JQueryTag jqt)
        {
            this._subset = new List<JQueryTag>();
            this._subset.Add(jqt);
            this._selector = "jQuery(" + jqt._selector + ")";
            this._iterator = 0;
            this._js = jqt.getJS();
        }

        /// <summary>
        /// Initializes a new JQuerySelector
        /// </summary>
        /// <param name="js">Used to execute javaScript code.</param>
        /// <param name="selector">A string containing a selector expression used for the filtered elements.</param>
        /// <param name="subset">Contains the webElements used to create JQueryTags</param>
        public JQuerySelector(IJavaScriptExecutor js, string selector, List<IWebElement> subset)
        {
            this._selector = selector;
            this._iterator = 0;
            this._subset = new List<JQueryTag>();
            this._js = js;

            for (int i = 0; i < subset.Count; i++)
            {
                try
                {
                    this._subset.Add(new JQueryTag(js, selector, i, subset[i]));
                }
                catch (StaleElementReferenceException)
                {
                }
            }
        }
        #endregion

        #region add()
        /// <summary>
        ///  Add elements to the set of matched elements.
        /// <para>Source: http://api.jquery.com/add/ </para>
        /// </summary>
        /// <param name="selector_elements_html">
        /// <para>-selector: A string representing a selector expression to find additional elements to add 
        /// to the set of matched elements.</para>
        /// <para>-elements_ One or more elements to add to the set of matched elements.</para>
        /// <para>-html: An HTML fragment to add to the set of matched elements.</para>
        /// </param>
        /// <returns>JQuerySelector</returns>
        public JQuerySelector add(string selector_elements_html)
        {
            Object result;
            string newSelector;
            if (requiresApostrophe(selector_elements_html))
            {
                result = execJS("jQuery(", ").add('" + selector_elements_html + "');");
                newSelector = "jQuery(" + _selector + ").add('" + selector_elements_html + "')";
            }
            else
            {
                result = execJS("jQuery(", ").add(" + selector_elements_html + ");");
                newSelector = "jQuery(" + _selector + ").add(" + selector_elements_html + ")";
            }
            _subset = objectToJQueryTagList(result);
            overwriteSelectors(newSelector);
            return this;
        }

        /// <summary>
        ///  Add elements to the set of matched elements.
        /// <para>Source: http://api.jquery.com/add/ </para>
        /// </summary>
        /// <param name="selector">
        /// A string representing a selector expression to find additional elements to add to 
        /// the set of matched elements.
        /// </param>
        /// <param name="context">
        /// The point in the document at which the selector should begin matching; similar to 
        /// the context argument of the $(selector, context) method.
        /// </param>
        /// <returns>JQuerySelector</returns>
        public JQuerySelector add(string selector, string context)
        {
            Dictionary<string, Object> result =
                (Dictionary<string, Object>) execJS("jQuery(", ").add('" + selector + "'," + context + ");");
            string newSelector = "jQuery(" + this._selector + ").add('" + selector + "'," + context + ")";
            _subset = objectToJQueryTagList(result);
            overwriteSelectors(newSelector);
            return this;
        }
        #endregion

        #region addClass()
        /// <summary>
        /// Adds the specified class(es) to each of the set of matched elements.
        /// <para>Source: http://api.jquery.com/addClass/ </para>
        /// </summary>
        /// <param name="className_function"><para>-className: One or more class names to be 
        /// added to the class attribute of each matched element.</para>
        /// <para>-function(index, currentClass): A function returning one or more space-separated 
        /// class names to be added to the existing class name(s). Receives the index position of 
        /// the element in the set and the existing class name(s) as arguments. Within the function, 
        /// this refers to the current element in the set.</para></param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector addClass(string className_function)
        {
            if (requiresApostrophe(className_function))
            {
                execJS("jQuery(", ").addClass('" + className_function + "');");
            }
            else
            {
                execJS("jQuery(", ").addClass(" + className_function + ");");
            }
            return this;
        }
        #endregion

        #region after()
        /// <summary>
        /// Insert content, specified by the parameter, after each element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/after/ </para>
        /// </summary>
        /// <param name="content_content"><para>-content: HTML string, DOM element, or jQuery object to insert after 
        /// each element in the set of matched elements.</para>
        /// <para>-content: One or more additional DOM elements, arrays of elements, HTML strings, or jQuery objects to 
        /// insert after each element in the set of matched elements.</para></param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector after(params string[] content_content)
        {
            Object result;
            string resultingContent = "";
            foreach (var s in content_content)
            {
                if (requiresApostrophe(s))
                {
                    resultingContent += "'" + s + "',";
                }
                else
                {
                    resultingContent += s + ",";
                }
            }
            resultingContent = resultingContent.Remove(resultingContent.Length - 1);
            Console.WriteLine(resultingContent);
            result = execJS("jQuery(", ").after(" + resultingContent + ");");
            _subset = objectToJQueryTagList(result);
            return this;
        }
        #endregion 

        #region append()
        /// <summary>
        /// Insert content, specified by the parameter, to the end of each element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/append/ </para>
        /// </summary>
        /// <param name="content_content">
        /// <para>-content: DOM element, HTML string, or jQuery object to insert at the end of each element in the set 
        /// of matched elements.</para>
        /// <para>-content: One or more additional DOM elements, arrays of elements, HTML strings, or jQuery objects to 
        /// insert at the end of each element in the set of matched elements.</para>
        /// </param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector append(params string[] content_content)
        {
            Object result;
            string resultingContent = "";
            foreach (var s in content_content)
            {
                if (requiresApostrophe(s))
                {
                    resultingContent += "'" + s + "',";
                }
                else
                {
                    resultingContent += s + ",";
                }
            }
            resultingContent = resultingContent.Remove(resultingContent.Length - 1);
            Console.WriteLine(resultingContent);
            result = execJS("jQuery(", ").append(" + resultingContent + ");");
            _subset = objectToJQueryTagList(result);
            return this;
        }
        #endregion 

        #region appendTo()
        /// <summary>
        /// Insert every element in the set of matched elements to the end of the target.
        /// <para>Source: http://api.jquery.com/appendTo/ </para>
        /// </summary>
        /// <param name="target">A selector, element, HTML string, or jQuery object; the matched set of 
        /// elements will be inserted at the end of the element(s) specified by this parameter.</param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector appendTo(string target)
        {
            if (requiresApostrophe(target))
            {
                execJS("jQuery(", ").appendTo('" + target + "');");
            }
            else
            {
                execJS("jQuery(", ").appendTo(" + target + ");");
            }
            return this;
        }
        #endregion 

        #region attr()
        /// <summary>
        /// Get the value of an attribute for the first element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/attr/#attr1 </para>
        /// </summary>
        /// <param name="attribute_name">The name of the attribute to get.</param>
        /// <returns>String containing the element's attribute value.</returns>
        public string attr(string attribute_name)
        {

            return _subset[0]._webElement.GetAttribute(attribute_name);
        }
        
        /// <summary>
        /// Set one or more attributes for the set of matched elements.
        /// <para>Source: http://api.jquery.com/attr/#attr2 </para>
        /// </summary>
        /// <param name="attribute_name">The name of the attribute to set.</param>
        /// <param name="new_value">A value to set for the attribute.</param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector attr(string attribute_name, string new_value)
        {
            if (requiresApostrophe(new_value))
            {
                execJS("jQuery(", ").attr(\"" + attribute_name + "\",'" + new_value + "');");
            }
            else
            {
                execJS("jQuery(", ").attr(\"" + attribute_name + "\"," + new_value + ");");
            }
            return this;
        }
        #endregion

        #region css()
        /// <summary>
        /// Get the value of a style property for the first element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/css/#css1 </para>
        /// </summary>
        /// <param name="css_property">A CSS property.</param>
        /// <returns>String containing the CSS property value.</returns>
        public string css (string css_property)
        {
            return _subset[0].css(css_property);
        }
        
        /// <summary>
        /// Set one or more CSS properties for the set of matched elements.
        /// <para>Source: http://api.jquery.com/css/#css2 </para>
        /// </summary>
        /// <param name="css_property">A CSS property name.</param>
        /// <param name="new_value">A value to set for the property.</param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector css(string css_property, string new_value)
        {
            if (requiresApostrophe(new_value))
            {
                execJS("jQuery(", ").css(\"" + css_property + "\",'" + new_value + "');");
            }
            else
            {
                execJS("jQuery(", ").css(\"" + css_property + "\"," + new_value + ");");
            }
            return this;
        }
        #endregion

        #region execJS()
        ///<summary>
        /// Executes a javascript function by concatenating a prefix and a suffix to the selector of the JQuerySelector.
        ///</summary>
        /// <param name="prefix">It represents all the javascript code that goes before the selector.</param>
        /// <param name="suffix">It represents all the javascript code that goes after the selector.</param>
        private Object execJS(string preffix, string suffix)
        {
            Console.WriteLine("return " + preffix + this._selector + suffix);
            return _js.ExecuteScript("return " + preffix + this._selector + suffix);
        }
        #endregion

        #region get()
        /// <summary>
        /// Returns the next element in the JQuerySelector.
        /// </summary>
        /// <returns>JQueryTag containing a webElement</returns>
        public JQueryTag Get()
        {
            return _subset[_iterator++];
        }

        /// <summary>
        /// Returns the element with the provided index from the JQuerySelector.
        /// </summary>
        /// <param name="index">Position of the element in the JQuerySelector.</param>
        /// <returns>JQueryTag containing a webElement.</returns>
        public JQueryTag Get(int index)
        {
            if(index < _subset.Count)
            {
                return _subset[index];
            }    
            return null;
        }
        #endregion 

        #region hasClass()
        /// <summary>
        /// Determine whether any of the matched elements are assigned the given class.
        /// <para>Source: http://api.jquery.com/hasClass/ </para>
        /// </summary>
        /// <param name="className">The class name to search for.</param>
        /// <returns>True if anly element has the provided className.
        /// <para>False if none of them has the provided className.</para></returns>
        public bool hasClass(string className)
        {
            foreach (JQueryTag element in _subset)
            {
                if (element._webElement.GetAttribute("class").Contains(className))
                    return true;
            }
            return false;
        }
        #endregion

        #region html()
        /// <summary>
        /// Get the HTML contents of the first element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/html/#html1 </para>
        /// </summary>
        /// <returns>A string containing the HTML contents of the first element in the set of matched 
        /// elements.</returns>
        public string html ()
        {
            return execJS("jQuery(", ").html()").ToString();
        }

        /// <summary>
        /// Set the HTML contents of each element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/html/#html2 </para>
        /// </summary>
        /// <param name="htmlString">A string of HTML to set as the content of each matched element.</param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector html(string htmlString)
        {
            Object result = execJS("jQuery(", ").html('" + htmlString + "')");
            Console.WriteLine(result.ToString());
            this._subset = objectToJQueryTagList(result);
            return this;
        }
        #endregion

        #region isEmpty()
        /// <summary>
        /// Determines if the JQuerySelector contains any elements
        /// </summary>
        /// <returns>True if it does not contain any elements.
        /// <para>False if it contains elements.</para></returns>
        public bool isEmpty()
        {
            if (_subset.Count == 0)
                return true;
            return false;
        }
        #endregion 

        #region objectToJQueryTagList()
        /// <summary>
        /// Converts an object returned from a javaScript function into a list of JQueryTags
        /// </summary>
        /// <param name="result">The result of the javaScript code.</param>
        /// <returns>A list containing all JQueryTags</returns>
        private List<JQueryTag> objectToJQueryTagList(Object result)
        {
            List<JQueryTag> jQueryTags = new List<JQueryTag>();
            if (result.GetType() == (new Dictionary<string, Object>()).GetType())
            {
                Dictionary<string, Object> dictionary = (Dictionary<string, Object>)result;
                int length = Convert.ToInt32(dictionary["length"]);
                for (int i = 0; i < length; i++)
                {
                    jQueryTags.Add(new JQueryTag(_js, _selector, i, (IWebElement)dictionary[Convert.ToString(i)]));
                }
            }
            else
            {
                List<IWebElement> webElements = new List<IWebElement>(((ReadOnlyCollection<IWebElement>)result));
                for (int i = 0; i < webElements.Count; i++)
                {
                    jQueryTags.Add(new JQueryTag(_js, _selector, i, webElements[i]));
                }
            }
            return jQueryTags;
        }
        #endregion 

        #region overwriteSelectors()
        /// <summary>
        /// Overwrites all of the selectors of each of the JQueryTags and the JQuerySelector
        /// </summary>
        /// <param name="selector">The new selector for the JQuerySelector and its JQueryTags</param>
        public void overwriteSelectors(string selector)
        {
            this._selector = selector;
            for (int i = 0; i < _subset.Count; i++)
            {
                _subset[i]._selector = selector +"["+i+"]";
            }
        }
        #endregion 

        #region remove()
        /// <summary>
        /// Remove the set of matched elements from the DOM.
        /// <para>Source: http://api.jquery.com/remove/ </para>
        /// </summary>
        public void remove()
        {
            execJS("jQuery(", ").remove()");
        }

        /// <summary>
        /// Remove the set of matched elements from the DOM.
        /// <para>Source: http://api.jquery.com/remove/ </para>
        /// </summary>
        /// <param name="selector">A selector expression that filters the set of matched elements to be 
        /// removed.</param>
        public void remove (string selector)
        {
            execJS("jQuery(", ").remove('" + selector + "')");
        }
        #endregion 

        #region requiresApostrophe()
        /// <summary>
        /// Determines if a parameter of a javaScript function requires apostrophes around it.
        /// </summary>
        /// <param name="parameter">The parameter of a javaScript function</param>
        /// <returns>True if it requires to be wrapped in apostrophes.
        /// <para>False if it doesn't require to be wrapped in apostrophes.</para></returns>
        public bool requiresApostrophe(string parameter)
        {
            if (parameter.Split('(')[0].Contains("function") || parameter.Split('.')[0].Contains("document")
                || parameter.Split('(')[0].Contains("$") || parameter.Split('(')[0].Contains("jQuery"))
            {
                return false;
            }
            return true;
        }
        #endregion 

        #region text()
        /// <summary>
        /// Get the combined text contents of each element in the set of matched elements, including their descendants.
        /// <para>Source: http://api.jquery.com/text/#text1 </para>
        /// </summary>
        /// <returns>A string containing the text of an HTML element.</returns>
        public string text()
        {
            Object result = execJS("jQuery(", ").text();");
            string resultText = result.ToString();
            return resultText;
        }

        /// <summary>
        ///  Set the content of each element in the set of matched elements to the specified text.
        /// <para>Source: http://api.jquery.com/text/#text2 </para>
        /// </summary>
        /// <param name="textString_function">
        /// <para>textString A string of text to set as the content of each matched element.</para>
        /// <para>function(index, text) A function returning the text content to set. 
        /// Receives the index position of the element in the set and the old text value as arguments.</para>
        /// </param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector text(string textString_function)
        {
            Object result;
            if (requiresApostrophe(textString_function))
            {
                result = execJS("jQuery(", ").text('" + textString_function + "');");
            }
            else
            {
                result = execJS("jQuery(", ").text(" + textString_function + ");");
            }
            _subset = objectToJQueryTagList(result);
            return this;
        }
        #endregion 

        #region val()
        /// <summary>
        /// Get the current value of the first element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/val/#val1 </para>
        /// </summary>
        /// <returns>A string containing the current value of the first element in the set of matched elements.
        /// </returns>
        public string val()
        {
            return execJS("jQuery(", ").val();").ToString();
        }

        /// <summary>
        /// Set the value of each element in the set of matched elements.
        /// <para>Source: http://api.jquery.com/val/#val2 </para>
        /// </summary>
        /// <param name="value">A string of text or an array of strings corresponding to the value of each 
        /// matched element to set as selected/checked.</param>
        /// <returns>JQuerySelector containing the modified elements.</returns>
        public JQuerySelector val(string value)
        {
            Object result = execJS("jQuery(", ").val('"+value+"');");
            this._subset = objectToJQueryTagList(result);
            return this;
        }
        #endregion 

        #region IEnumerator
        public IEnumerator<JQueryTag> GetEnumerator()
        {
            for (int i = 0; i < _subset.Count; i++)
            {
                yield return _subset[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion 
    }
}