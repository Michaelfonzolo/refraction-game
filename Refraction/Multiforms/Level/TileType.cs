#region License

// Copyright (c) 2015 FCDM
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region Header

/* Author: Michael Ala
 * 
 * Description
 * ===========
 */

#endregion

namespace Refraction_V2.Multiforms.Level
{

	/// <summary>
	/// The possible tile types.
	/// </summary>
    public enum TileType
    {
        Empty,
        Outputter,
        Receiver,
        OpenReceiver,
        OpenReceiver_pass_L,
        OpenReceiver_pass_U,
        OpenReceiver_pass_UL,
        OpenReceiver_pass_UR,
        RF_UxL_UL,
        RF_UxR_UR,
        RF_DxL_DL,
        RF_DxR_DR,
        RF_ULxUR_U,
        RF_DLxDR_D,
        RF_ULxDL_L,
        RF_URxDR_R,
        RF_U_L_and_U_R_pass_UL,
        RF_U_L_and_U_R_pass_UR,
        RF_UL_DL_and_UR_DR_pass_L,
        RF_UL_UR_and_DL_DR_pass_U
    }
}
