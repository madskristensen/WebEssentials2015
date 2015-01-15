using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.CSS.Core;
using Microsoft.VisualStudio.Utilities;
using Microsoft.CSS.Core.TreeItems;
using Microsoft.Web.Languages.Less.Parser;
using Microsoft.CSS.Editor.Completion;
using Microsoft.CSS.Core.TreeItems.Selectors;
using Microsoft.CSS.Core.Parser;

namespace MadsKristensen.EditorExtensions.Css
{
	[Export(typeof(ICssCompletionContextProvider))]
	[Name("LessPseudoContextProvider")]
	[Order(Before = "Default Pseudo")]
	internal class LessPseudoContextProvider : ICssCompletionContextProvider
	{
		public IEnumerable<Type> ItemTypes
		{
			get
			{
				return new Type[]
				{
					typeof(PseudoClassFunctionSelector),
					typeof(PseudoClassSelector),
					typeof(PseudoElementFunctionSelector),
					typeof(PseudoElementSelector)
				};
			}
		}

		public CssCompletionContext GetCompletionContext(ParseItem item, int position)
		{
			RuleSet rule = item.FindType<RuleSet>();

			if (rule != null && rule.Parent is LessRuleBlock)
			{
				return new CssCompletionContext(CssCompletionContextType.Invalid, item.Start, item.Length, item);
			}

			return null;
		}
	}
}