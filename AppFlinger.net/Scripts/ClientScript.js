//--------------------------------------------------------------------------
// <copyright file="ClientScript.js" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    This JScript file provides JScript function signatures exposed by the client.
//    To display these functions in the intellisense, follow the below steps:
//      a. Drag and drop a TVScript control on the page.
//      b. In the <script> tag within the control set the Src="~/bin/Resources/ClientScript.js".
//      c. After adding an external JScript file path in Src attribute, if the intellisense doesn't
//         show the JScript function, press Shift+Ctrl+J to refresh the JScript intellisense.
//
//    Example:
//      <mrml:TVScript ID="TVScript1" runat="server" style="top: 46; left: 71">
//        <Script Src="Bin/Resources/ClientScript.js">      
//        </Script>
//      </mrml:TVScript>
//
//    To display the ClientScript.js functions into intellisense of the other external JScript file
//    either
//       a. drag and drop the "ClientScript.js" file from solution explorer to the another JScript file on the 1st line, or
//       b. add the following line on the top i.e. first line
//              /// <reference path="Bin/Resources/ClientScript.js" />
//
//       The step (a) automatically generate the line mentioned in step (b).
//
//    Note:
//      1. Don't change the content of ClientScript.js file.
//      2. The Script control will not render the src attribute, if it is pointing to 
//         ClientScript.js file.
// </summary>
//--------------------------------------------------------------------------

function getProperty(id, property) {
    /// <summary>
    /// getProperty method gets the value of the specified property on the given gadget.
    /// The properties whose value function can return are left, top, width, height, alpha, text, foreground, visible.
    /// </summary>
    /// <param name="id" type="string">The id associated with the target gadget.</param>
    /// <param name="property" type="string">The requested property of the gadget.</param>
    /// <returns type="object">Return the property value of the gadget as a string object.</returns>
}

/*
function getProperty(id, xpath) {
    /// <summary>
    /// Use getProperty on a DataSource element with XPath query to refer to the XML document being bound to the data source. See data binding document for more info. When use getProperty to access data, it can only be used to find a specific node/attribute. Multiple selections are not supported.    
    /// </summary>
    /// <param name="id" type="string">The id of the DataSource element.</param>
    /// <param name="xpath" type="string">The XPath query for the XML data source.</param>
    /// <returns type="object">Return the reference value of the XPath query.</returns>
}
*/

function setProperty(id, property, value) {
    /// <summary>
    /// setProperty method sets the value of the specified property on the given gadget.
    /// The properties which can be set by the function are left, top, width, height, alpha, text, foreground, visible.
    /// </summary>
    /// <param name="id" type="string">The id associated with the target gadget.</param>
    /// <param name="property" type="string">The requested property of the gadget.</param>
    /// <param name="value" type="string">A string that specifies the property to set.</param>
}

function invokeAction(id, action) {
    /// <summary>
    /// invokeAction method executes the specified action in the scope of a gadget or page. If the gadget id is null or empty, it will execute the specified action from the page scope.
    /// </summary>
    /// <param name="id" type="string">The gadget id that the specified action is under.</param>
    /// <param name="action" type="string">The name of the action to be executed.</param>
}

function setTimeout(script, delay)
{
    /// <summary>
    /// setTimeout method executes the script after the specified interval has elapsed. Dynamic Actions does not support Window object which implemented the setTimeout property. So setTimeout method here is exposed from the presentation framework directly.
    /// </summary>
    /// <param name="script" type="string">A string that contains the The JavaScript code to be executed after the delay has elapsed.</param>
    /// <param name="delay" type="string">An integer value that specify the amount of time in milliseconds before the script should be executed. </param>
    /// <returns type="string">Return an opaque value as timeout id.</returns>
}

function clearTimeout(timeoutId)
{
    /// <summary>
    /// clearTimeout method cancels the execution of code that has been deferred with the setTimeout method. 
    /// </summary>
    /// <param name="timeoutId" type="string">A value returned by setTimeout that identifies the timeout to be canceled.</param>
}
