public static class StringUtils
{
	public static string RemoveRichText(string input)
	{
		input = RemoveRichTextDynamicTag(input, "color");
		input = RemoveRichTextTag(input, "b");
		input = RemoveRichTextTag(input, "i");
		input = RemoveRichTextDynamicTag(input, "align");
		input = RemoveRichTextDynamicTag(input, "size");
		input = RemoveRichTextDynamicTag(input, "cspace");
		input = RemoveRichTextDynamicTag(input, "font");
		input = RemoveRichTextDynamicTag(input, "indent");
		input = RemoveRichTextDynamicTag(input, "line-height");
		input = RemoveRichTextDynamicTag(input, "line-indent");
		input = RemoveRichTextDynamicTag(input, "link");
		input = RemoveRichTextDynamicTag(input, "margin");
		input = RemoveRichTextDynamicTag(input, "margin-left");
		input = RemoveRichTextDynamicTag(input, "margin-right");
		input = RemoveRichTextDynamicTag(input, "mark");
		input = RemoveRichTextDynamicTag(input, "mspace");
		input = RemoveRichTextDynamicTag(input, "noparse");
		input = RemoveRichTextDynamicTag(input, "nobr");
		input = RemoveRichTextDynamicTag(input, "page");
		input = RemoveRichTextDynamicTag(input, "pos");
		input = RemoveRichTextDynamicTag(input, "space");
		input = RemoveRichTextDynamicTag(input, "sprite index");
		input = RemoveRichTextDynamicTag(input, "sprite name");
		input = RemoveRichTextDynamicTag(input, "sprite");
		input = RemoveRichTextDynamicTag(input, "style");
		input = RemoveRichTextDynamicTag(input, "voffset");
		input = RemoveRichTextDynamicTag(input, "width");
		input = RemoveRichTextTag(input, "u");
		input = RemoveRichTextTag(input, "s");
		input = RemoveRichTextTag(input, "sup");
		input = RemoveRichTextTag(input, "sub");
		input = RemoveRichTextTag(input, "allcaps");
		input = RemoveRichTextTag(input, "smallcaps");
		input = RemoveRichTextTag(input, "uppercase");
		return input;
	}

	private static string RemoveRichTextDynamicTag(string input, string tag)
	{
		int num = -1;
		while (true)
		{
			num = input.IndexOf("<" + tag + "=");
			if (num == -1)
			{
				break;
			}
			int num2 = input.Substring(num, input.Length - num).IndexOf('>');
			if (num2 > 0)
			{
				input = input.Remove(num, num2 + 1);
			}
		}
		input = RemoveRichTextTag(input, tag, isStart: false);
		return input;
	}

	private static string RemoveRichTextTag(string input, string tag, bool isStart = true)
	{
		while (true)
		{
			int num = input.IndexOf(isStart ? ("<" + tag + ">") : ("</" + tag + ">"));
			if (num == -1)
			{
				break;
			}
			input = input.Remove(num, 2 + tag.Length + (!isStart).GetHashCode());
		}
		if (isStart)
		{
			input = RemoveRichTextTag(input, tag, isStart: false);
		}
		return input;
	}
}
