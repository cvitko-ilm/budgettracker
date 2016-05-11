﻿using System;
using SQLite;

namespace BudgetTracker
{
	public class Category
	{
		public Category ()
		{
		}

		public string Name {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		[PrimaryKey]
		public string Id {
			get;
			set;
		}

		public CategoryType CategoryType {
			get;
			set;
		}
	}
}

