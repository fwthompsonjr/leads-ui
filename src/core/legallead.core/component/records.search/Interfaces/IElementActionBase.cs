using legallead.records.search.Classes;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Interfaces
{
    public interface IElementActionBase
    {
        /// <value>
        /// The name of the action.
        /// </value>
        string ActionName { get; }

        /// <summary>
        /// Gets or sets the get assertion.
        /// </summary>
        /// <value>
        /// The get assertion.
        /// </value>
        ElementAssertion GetAssertion { get; set; }

        /// <summary>
        /// Gets or sets the get web.
        /// </summary>
        /// <value>
        /// The get web.
        /// </value>
        IWebDriver GetWeb { get; set; }

        /// <summary>
        /// Acts the specified step.
        /// </summary>
        /// <param name="navigationStep">The navigation step.</param>
        void Act(NavigationStep item);

        /// <summary>
        /// Gets or sets the outer HTML.
        /// </summary>
        /// <value>
        /// The outer HTML.
        /// </value>
        string OuterHtml { get; set; }

        WebNavigationParameter GetSettings(int index);
    }
}