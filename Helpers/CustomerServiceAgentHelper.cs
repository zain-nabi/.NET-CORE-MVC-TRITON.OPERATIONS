namespace Triton.Operations.Helpers
{
    public class CustomerServiceAgentHelper
    {
        public static string GenerateEmailBody(string emailBody, string accountCode, string name)
        {
            var body = $@"<p style='margin-bottom:12.0pt'><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,sans-serif'>{emailBody}<br><br>The details are as follows:</span><span><o:p></o:p></span></p>
                                    <table border='0' cellspacing='0' cellpadding='0'>
                                       <tbody>
                                            <tr>
                                                <td width='186' style='width:139.5pt;padding:1.5pt 1.5pt 1.5pt 1.5pt'>
                                                    <p><strong><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>Account Code:</span></strong><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'><u></u><u></u></span></p>
                                                </td>
                                                <td width='228' style='width:171.0pt;padding:1.5pt 1.5pt 1.5pt 1.5pt'>
                                                    <p><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>{accountCode}<u></u><u></u></span></p>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width='186' style='width:139.5pt;padding:1.5pt 1.5pt 1.5pt 1.5pt'>
                                                    <p><strong><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>Company Name:</span></strong><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'><u></u><u></u></span></p>
                                                </td>
                                                <td width='228' style='width:171.0pt;padding:1.5pt 1.5pt 1.5pt 1.5pt'>
                                                    <p><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>{name}<u></u><u></u></span></p>
                                                </td>
                                            </tr>
                                    
                                        </tbody>
                                    </table>
                                    
                                    <p>
                                        <span>
                                            <br>
                                        </span><span style='font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>
                                         
                                            Regards,<br>
                                            <strong><span style='font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>Triton Express Operations</span></strong><b>
                                                <br>
                                                <br>
                                            </b>
                                        </span><span>
                                            <u></u><u></u>
                                        </span>
                                    </p>";
            return body;
        }

    }
}
