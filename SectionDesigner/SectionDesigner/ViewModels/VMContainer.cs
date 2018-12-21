﻿using System;
using System.Windows.Input;
using SectionDesigner.Models;
using SectionDesigner.Commands;

namespace SectionDesigner.ViewModels
{
	internal class VMContainer
	{
		public VMContainer() {
			
		VM1 = new SectionViewModel();
		VM2 = new LoadsViewModel();
		//VM3 = new TabItemViewModel();
		}
		
		public SectionViewModel VM1 {get; set;}
		public LoadsViewModel VM2 {get; set;}
		public TabItemViewModel VM3 {get; set;}
		
	}
}
