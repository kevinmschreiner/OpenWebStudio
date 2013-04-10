var $jq;
if (typeof jQuery!='undefined')
{
    if (typeof $dnn != 'undefined' && $dnn != null && $dnn.toString()==jQuery.toString())
        $jq=jQuery;
    else
        $jq=jQuery.noConflict();
}