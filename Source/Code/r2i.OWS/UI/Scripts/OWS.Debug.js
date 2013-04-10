function OWSDebugger()
{
    this.colorize = false;
	this.counter = 0;
	this.windows = new Array();
	this.listener = null;
	this.content = new Array();
	
	this.Toggle=function Toggle(id,ticks)
	{
		//OWSDebug.Toggle('" & Me.ModuleId & ",'" & ticks & "')
		var xdbgObj = document.getElementById('xDbg' + id + 'x' + ticks);
		if (xdbgObj)
		{
			if (xdbgObj.style.display == 'block')
			{
				xdbgObj.style.display = 'none';
			}
			else
			{
				xdbgObj.style.display = 'block';
			}
		}
	}
}
var OWSDebug = new OWSDebugger();



