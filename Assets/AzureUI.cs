﻿#region License
// Copyright (c) 2014 Bit Rave Pty Ltd
//
// 1. OWNERSHIP, LICENSE GRANT
// Subject to the terms below (the 'License Agreement'), Bit Rave Pty Ltd ('We', 'Us') 
// grants you to install and use Azure Mobile Services for Unity (the 'Software').
//
// of all intellectual property rights inherent in or relating to the Software, 
// which include, but are not limited to, all copyright, patent rights, all rights 
// in relation to registered and unregistered trademarks (including service marks), 
// confidential information (including trade secrets and know-how) and all rights 
// other than those expressly granted by this Agreement.
//
// Subject to the terms and conditions of this License Agreement, We grant to You 
// a non-transferable, non-exclusive license for a Designated User (as defined below) 
// within Your organization to install and use the Software on any workstations used 
// exclusively by such Designated User and for You to distribute the Software as part 
// of your Unity applications or games, solely in connection with distribution of 
// the Software in accordance with sections 3 and 4 below. This license is not 
// sublicensable except as explicitly set forth herein. "Designated User(s)" shall 
// mean Your employee(s) acting within the scope of their employment or Your consultant(s) 
// or contractor(s) acting within the scope of the services they provide for You or on Your behalf.

// 2. PERMITTED USES, SOURCE CODE, MODIFICATIONS
// We provide You with source code so that You can create Modifications of the original Software, 
// where Modification means: a) any addition to or deletion from the contents of a file included 
// in the original Software or previous Modifications created by You, or b) any new file that 
// contains any part of the original Software or previous Modifications. While You retain all 
// rights to any original work authored by You as part of the Modifications, We continue to own 
// all copyright and other intellectual property rights in the Software.

// 3. DISTRIBUTION
// You may distribute the Software in any applications, frameworks, or elements (collectively 
// referred to as "Applications") that you develop using the Software in accordance with this 
// License Agreement, provided that such distribution does not violate the restrictions set 
// forth in section 4 of this agreement.

// You will not owe Us any royalties for Your distribution of the Software in accordance with 
// this License Agreement.

// 4. PROHIBITED USES
// You may not redistribute the Software or Modifications other than by including the Software 
// or a portion thereof within Your own product, which must have substantially different 
// functionality than the Software or Modifications and must not allow any third party to use 
// the Software or Modifications, or any portions thereof, for software development or application 
// development purposes. You are explicitly not allowed to redistribute the Software or 
// Modifications as part of any product that can be described as a development toolkit or library 
// or is intended for use by software developers or application developers and not end-users.

// 5. TERMINATION
// This Agreement shall terminate automatically if you fail to comply with the limitations 
// described in this Agreement. No notice shall be required to effectuate such termination. 
// Upon termination, you must remove and destroy all copies of the Software. 

// 6. DISCLAIMER OF WARRANTY
// YOU AGREE THAT WE HAVE MADE NO EXPRESS WARRANTIES, ORAL OR WRITTEN, TO YOU REGARDING THE 
// SOFTWARE AND THAT THE SOFTWARE IS BEING PROVIDED TO YOU 'AS IS' WITHOUT WARRANTY OF ANY KIND.
//  WE DISCLAIM ANY AND ALL OTHER WARRANTIES, WHETHER EXPRESSED, IMPLIED, OR STATUTORY. YOUR RIGHTS
//  MAY VARY DEPENDING ON THE STATE IN WHICH YOU LIVE. WE SHALL NOT BE LIABLE FOR INDIRECT, 
// INCIDENTAL, SPECIAL, COVER, RELIANCE, OR CONSEQUENTIAL DAMAGES RESULTING FROM THE USE OF THIS PRODUCT.

// 7. LIMITATION OF LIABILITY
// YOU USE THIS PROGRAM SOLELY AT YOUR OWN RISK. IN NO EVENT SHALL WE BE LIABLE TO YOU FOR ANY DAMAGES,
// INCLUDING BUT NOT LIMITED TO ANY LOSS, OR OTHER INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF 
// ANY KIND ARISING OUT OF THE USE OF THE SOFTWARE, EVEN IF WE HAVE BEEN ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGES. IN NO EVENT WILL WE BE LIABLE FOR ANY CLAIM, WHETHER IN CONTRACT, TORT, OR ANY OTHER
// THEORY OF LIABILITY, EXCEED THE COST OF THE SOFTWARE. THIS LIMITATION SHALL APPLY TO CLAIMS OF 
// PERSONAL INJURY TO THE EXTENT PERMITTED BY LAW.

// 8. MISCELLANEOUS
// The license granted herein applies only to the version of the Software available when acquired
// in connection with the terms of this Agreement. Any previous or subsequent license granted to
// You for use of the Software shall be governed by the terms and conditions of the agreement entered
// in connection with the acquisition of that version of the Software. You agree that you will comply
// with all applicable laws and regulations with respect to the Software, including without limitation
// all export and re-export control laws and regulations.
#endregion
/*
 * Modified by Dave Voyles, Microsoft Corporation, July 2014
 */

using UnityEngine;
using System.Collections.Generic;
using Bitrave.Azure;

public class AzureUI : MonoBehaviour {

    private Vector2              _scrollPosition;
    private LeaderBoard          _score         = new LeaderBoard() { UserName="", Id=null, Score=0 };
    private List<LeaderBoard>    _leaders       = new List<LeaderBoard>();
    private AzureMobileServices  _azure;

    /* 
     * You can find both of these properties in your Azure Portal: https://manage.windowsazure.com
     * I serialized them, to expose them to the editor.
     * I chose not to hard code them, so that I can edit them within the Unity editor.
     */
    [SerializeField]
    private string               _azureEndPoint = "<your leaderboard service>";
    [SerializeField]
    private string               _applicationKey = "<your application key>";


    /// <summary>
    /// Creates a new Mobile Service by connecting to your Azure back end. 
    /// </summary>
	void Start () {
        _azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
    }


    /// <summary>
    /// Clears the current list of leaders, and returns all of the items from your Azure leaderboard
    /// </summary>
    public void GetAllItems()
    {
        _leaders.Clear();
        _azure.Where<LeaderBoard>(p => p.UserName != null, ReadHandler);
    }


    /// <summary>
    /// Loops through each item in the list and:
    /// 1) Draws them to the debug log
    /// 2) Add them to the leaderboard
    /// </summary>
    /// <param name="response">Pass in the leaderboard from Azure</param>
    public void ReadHandler(AzureResponse<List<LeaderBoard>> response)
    {
        var list = response.ResponseData;

        foreach (var item in list)
        {
            Debug.Log("Item:" + " " + item.ToString());
            _leaders.Add(item);
        }
    }
	 

    /// <summary>
    /// Draws the GUI text and buttons
    /// </summary>
    public void OnGUI()
    {
        // Container to hold the UI
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

            DrawLeftColumn();
            DrawCenterColumn();
            DrawListColumn();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.enabled = true;
    }



    /*
     * DRAWING COLUMNS  
     ************************************************/
    private void DrawCenterColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(200));
            _score.UserName = GUILayout.TextField(_score.UserName);
            _score.Score = int.Parse(GUILayout.TextField(_score.Score.ToString()));
            if (GUILayout.Button("Add"))
            {
                // Note: You don't need to do the following, 
                // it's done in the insert method. 
                _azure.Insert<LeaderBoard>(_score);
            }
        GUILayout.EndVertical();
    }


    private void DrawLeftColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Azure End Point");
            _azureEndPoint = GUILayout.TextField(_azureEndPoint, GUILayout.Width(200));
            GUILayout.Label("Application Key");
            _applicationKey = GUILayout.TextField(_applicationKey, GUILayout.Width(200));
        GUILayout.EndVertical();
    }


    private void DrawColumnForLeaderboardList()
    {
        GUILayout.BeginVertical();
        foreach (var item in _leaders)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.ToString());
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }


    private void DrawListColumn()
    {
        GUILayout.BeginVertical(GUILayout.Width(200));

        // Get all of the items currently stored on the leaderboard
        if (GUILayout.Button("List All Items On Leaderboard"))
        {
            GetAllItems();
        }

        GUILayout.Label("Item count: " + _leaders.Count);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Height(300));

        DrawColumnForLeaderboardList();
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }
  
}
