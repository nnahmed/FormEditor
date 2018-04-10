﻿using System.Collections.Generic;
using System.Linq;
using FormEditor.Fields;
using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace FormEditor.Validation
{
	public class Validation
	{
		[JsonIgnore]
		public virtual bool Invalid { get; set; }

		public IEnumerable<Rule> Rules { get; set; }

		public string ErrorMessage { get; set; }

		public bool IsValidFor(IEnumerable<FieldWithValue> allCollectedFieldValues, IPublishedContent content)
		{
			// swap the rule fields for the actual fields collected by the form model
			foreach (var rule in Rules)
			{
				if (rule.Field == null)
				{
					// should not happen!
					continue;
				}
				var collectedField = allCollectedFieldValues.FirstOrDefault(f => f.Name == rule.Field.Name);
				if (collectedField != null)
				{
					rule.Field = collectedField;
				}
			}

			Invalid = Rules.Any(r => r.IsApplicable == false)
				// it's impossible to validate the rule if we have frontend only conditions in play
				? false
				// the validation fails if all rules are fulfilled
				: Rules.All(r => r.IsFulfilledBy(allCollectedFieldValues, content));

			return Invalid == false;
		}
	}
}
