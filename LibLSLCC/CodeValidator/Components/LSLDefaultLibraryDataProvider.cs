using System;
using System.Collections.Generic;
using System.Xml;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     The LSLDefaultLibraryDataProvider reads XML from the embedded resource
    ///     LibLSLCC.CodeValidator.Components.LibraryData.StandardLSL.xml
    ///     to define its data
    /// </summary>
    [LSLXmlLibraryDataRoot]
    public class LSLDefaultLibraryDataProvider : LSLXmlLibraryDataProvider
    {
        protected LSLDefaultLibraryDataProvider()
        {
            
        }



        private HashSet<string> GetSubsets(LSLLibraryBaseData baseDataLSLLibraryBaseData, LSLLibraryDataAdditions dataAdditions)
        {
            var subsets=new HashSet<string>();
            if (baseDataLSLLibraryBaseData == LSLLibraryBaseData.All)
            {
                subsets.Add("all");
                this.AccumulateDuplicates = true;
                return subsets;
            }

            if (baseDataLSLLibraryBaseData == LSLLibraryBaseData.OpensimLsl)
            {
                subsets.Add("os-lsl");
            }
            else
            {
                subsets.Add("lsl");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimOssl) == LSLLibraryDataAdditions.OpenSimOssl)
            {
                subsets.Add("ossl");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimWindlight) == LSLLibraryDataAdditions.OpenSimWindlight)
            {
                subsets.Add("os-lightshare");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimBulletPhysics) == LSLLibraryDataAdditions.OpenSimBulletPhysics)
            {
                subsets.Add("os-bullet-physics");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimModInvoke) == LSLLibraryDataAdditions.OpenSimModInvoke)
            {
                subsets.Add("os-mod-api");
            }

            return subsets;
        }



        public LSLDefaultLibraryDataProvider(LSLLibraryBaseData libraryBaseData) : this(libraryBaseData,LSLLibraryDataAdditions.None)
        {
            
        }

        public LSLDefaultLibraryDataProvider(LSLLibraryBaseData libraryBaseData, LSLLibraryDataAdditions dataAdditions)
        {
            using (
                var libraryData =
                    GetType()
                        .Assembly.GetManifestResourceStream(
                            "LibLSLCC.CodeValidator.Components.LibraryData.LSLDefaultLibraryDataProvider.xml"))
            {
                if (libraryData == null)
                {
                    throw new InvalidOperationException(
                        "Could not locate manifest resource LibLSLCC.CodeValidator.Components.Resources.StandardLSLLibraryData.xml");
                }

                var reader = new XmlTextReader(libraryData);

                var subsets = GetSubsets(libraryBaseData, dataAdditions);

                FillFromXml(reader, subsets.AsReadOnly());




                #region OldDocBootstrap


                /*
                    GetLibraryConstantSignature("NULL_KEY").DocumentationString = "Indicates an empty key";
                    GetLibraryConstantSignature("EOF").DocumentationString =
                        "Indicates the last line of a notecard was read";
                    GetLibraryConstantSignature("TEXTURE_BLANK").DocumentationString =
                        "UUID for the &quot;Blank&quot; texture";
                    GetLibraryConstantSignature("TEXTURE_DEFAULT").DocumentationString =
                        "Alias for TEXTURE_PLYWOOD";
                    GetLibraryConstantSignature("TEXTURE_MEDIA").DocumentationString =
                        "UUID for the &quot;Default Media&quot; texture";
                    GetLibraryConstantSignature("TEXTURE_PLYWOOD").DocumentationString =
                        "UUID for the default &quot;Plywood&quot; texture";
                    GetLibraryConstantSignature("TEXTURE_TRANSPARENT").DocumentationString =
                        "UUID for the &quot;White - Transparent&quot; texture";

                    GetLibraryConstantSignature("URL_REQUEST_GRANTED").DocumentationString =
                        "Used with http_request when a public URL is successfully granted";
                    GetLibraryConstantSignature("URL_REQUEST_DENIED").DocumentationString =
                        "Used with http_request when a public URL is not available";
                    GetLibraryConstantSignature("PI").DocumentationString =
                        "3.1415926535897932384626433832795";
                    GetLibraryConstantSignature("TWO_PI").DocumentationString =
                        "6.283185307179586476925286766559";
                    GetLibraryConstantSignature("PI_BY_TWO").DocumentationString =
                        "1.5707963267948966192313216916398";
                    GetLibraryConstantSignature("DEG_TO_RAD").DocumentationString =
                        "To convert from degrees to radians";
                    GetLibraryConstantSignature("RAD_TO_DEG").DocumentationString =
                        "To convert from radians to degrees";
                    GetLibraryConstantSignature("SQRT2").DocumentationString =
                        "1.4142135623730950488016887242097";

                    GetLibraryConstantSignature("ZERO_VECTOR").DocumentationString =
                        "&lt;0.0, 0.0, 0.0&gt;";
                    GetLibraryConstantSignature("ZERO_ROTATION").DocumentationString =
                        "&lt;0.0, 0.0, 0.0, 1.0&gt;";



                    GetEventHandlerSignature("state_entry").DocumentationString =
                        "Triggered on any state transition and startup";
                    GetEventHandlerSignature("state_exit").DocumentationString =
                        "Triggered on any state transition";
                    GetEventHandlerSignature("touch_start").DocumentationString =
                        "Triggered by the start of agent clicking on task";
                    GetEventHandlerSignature("touch").DocumentationString =
                        "Triggered while agent is clicking on task";
                    GetEventHandlerSignature("touch_end").DocumentationString =
                        "Triggered when agent stops clicking on task";
                    GetEventHandlerSignature("collision_start").DocumentationString =
                        "Triggered when task starts colliding with another task";
                    GetEventHandlerSignature("collision").DocumentationString =
                        "Triggered while task is colliding with another task";
                    GetEventHandlerSignature("collision_end").DocumentationString =
                        "Triggered when task stops colliding with another task";
                    GetEventHandlerSignature("land_collision_start").DocumentationString =
                        "Triggered when task starts colliding with land";
                    GetEventHandlerSignature("land_collision").DocumentationString =
                        "Triggered when task is colliding with land";
                    GetEventHandlerSignature("land_collision_end").DocumentationString =
                        "Triggered when task stops colliding with land";
                    GetEventHandlerSignature("timer").DocumentationString =
                        "Result of the llSetTimerEvent library function call";
                    GetEventHandlerSignature("listen").DocumentationString =
                        "Result of the llListen library function call";
                    GetEventHandlerSignature("sensor").DocumentationString =
                        "Result of the llSensor library function call";
                    GetEventHandlerSignature("no_sensor").DocumentationString =
                        "Result of the llSensor library function call";
                    GetEventHandlerSignature("control").DocumentationString =
                        "Result of llTakeControls library function call";
                    GetEventHandlerSignature("at_target").DocumentationString =
                        "Result of llTarget library function call";
                    GetEventHandlerSignature("not_at_target").DocumentationString =
                        "Result of llTarget library function call";
                    GetEventHandlerSignature("at_rot_target").DocumentationString =
                        "Result of LLRotTarget library function call";
                    GetEventHandlerSignature("not_at_rot_target").DocumentationString =
                        "Result of LLRotTarget library function call";
                    GetEventHandlerSignature("money").DocumentationString =
                        "Triggered when L$ is given to task";
                    GetEventHandlerSignature("email").DocumentationString =
                        "Triggered when task receives email";
                    GetEventHandlerSignature("run_time_permissions").DocumentationString =
                        "Triggered when an agent grants run time permissions to task";
                    GetEventHandlerSignature("attach").DocumentationString =
                        "Triggered when task attaches or detaches from agent";
                    GetEventHandlerSignature("dataserver").DocumentationString =
                        "Triggered when task receives asynchronous data";
                    GetEventHandlerSignature("moving_start").DocumentationString =
                        "Triggered when task begins moving";
                    GetEventHandlerSignature("moving_end").DocumentationString =
                        "Triggered when task stops moving";
                    GetEventHandlerSignature("on_rez").DocumentationString =
                        "Triggered when task is rezzed in from inventory or another task";
                    GetEventHandlerSignature("object_rez").DocumentationString =
                        "Triggered when task rezzes in another task";
                    GetEventHandlerSignature("link_message").DocumentationString =
                        "Triggered when task receives a link message via LLMessageLinked library function call"
                            ;
                    GetEventHandlerSignature("changed").DocumentationString =
                        "Triggered various event change the task;(test change with CHANGED_INVENTORY, CHANGED_COLOR, CHANGED_SHAPE, CHANGED_SCALE, CHANGED_TEXTURE, CHANGED_LINK, CHANGED_ALLOWED_DROP, CHANGED_OWNER, CHANGED_REGION, CHANGED_TELEPORT, CHANGED_REGION_START, CHANGED_MEDIA)"
                            ;
                    GetEventHandlerSignature("remote_data").DocumentationString =
                        "Triggered by various XML-RPC calls (event_type will be one of REMOTE_DATA_CHANNEL, REMOTE_DATA_REQUEST, REMOTE_DATA_REPLY)"
                            ;
                    GetEventHandlerSignature("http_response").DocumentationString =
                        "Triggered when task receives a response to one of its llHTTPRequests";
                    GetEventHandlerSignature("http_request").DocumentationString =
                        "Triggered when task receives an http request against a public URL";
                    GetEventHandlerSignature("transaction_result").DocumentationString =
                        " Triggered when L$ is given to task";
                    GetEventHandlerSignature("path_update").DocumentationString =
                        "Informs the script of events that happen within the pathfinding system";

                    GetLibraryConstantSignature("TRUE").DocumentationString =
                        "integer_constant for Boolean operations";
                    GetLibraryConstantSignature("FALSE").DocumentationString =
                        "integer_constant for Boolean operations";
                    GetLibraryConstantSignature("STATUS_PHYSICS").DocumentationString =
                        "Passed in the llSetStatus library function.  If TRUE, object moves physically";
                    GetLibraryConstantSignature("STATUS_PHANTOM").DocumentationString =
                        "Passed in the llSetStatus library function.  If TRUE, object doesn't collide with other objects"
                            ;
                    GetLibraryConstantSignature("STATUS_ROTATE_X").DocumentationString =
                        "Passed in the llSetStatus library function.  If FALSE, object doesn't rotate around local X axis"
                            ;
                    GetLibraryConstantSignature("STATUS_ROTATE_Y").DocumentationString =
                        "Passed in the llSetStatus library function.  If FALSE, object doesn't rotate around local Y axis"
                            ;
                    GetLibraryConstantSignature("STATUS_ROTATE_Z").DocumentationString =
                        "Passed in the llSetStatus library function.  If FALSE, object doesn't rotate around local Z axis"
                            ;
                    GetLibraryConstantSignature("STATUS_SANDBOX").DocumentationString =
                        "Passed in the llSetStatus library function.  If TRUE, object can't cross region boundaries or move more than 10 meters from its start location"
                            ;
                    GetLibraryConstantSignature("STATUS_BLOCK_GRAB").DocumentationString =
                        "Passed in the llSetStatus library function.  If TRUE, root prim of linkset (or unlinked prim) can't be grabbed and physically dragged"
                            ;
                    GetLibraryConstantSignature("STATUS_DIE_AT_EDGE").DocumentationString =
                        "Passed in the llSetStatus library function.  If TRUE, objects that reach the edge of the world just die;rather than teleporting back to the owner"
                            ;
                    GetLibraryConstantSignature("STATUS_RETURN_AT_EDGE").DocumentationString =
                        " Passed in the llSetStatus library function.  If TRUE, script rezzed objects that reach the edge of the world;are returned rather than killed;STATUS_RETURN_AT_EDGE trumps STATUS_DIE_AT_EDGE if both are set"
                            ;
                    GetLibraryConstantSignature("STATUS_CAST_SHADOWS").DocumentationString =
                        "Passed in the llSetStatus library function.  If TRUE, object casts shadows on other objects"
                            ;
                    GetLibraryConstantSignature("STATUS_BLOCK_GRAB_OBJECT").DocumentationString =
                        "This status flag keeps the object from being moved by grabs. This flag applies to the entire linkset."
                            ;
                    GetLibraryConstantSignature("AGENT").DocumentationString =
                        "Passed in llSensor library function to look for other Agents";
                    GetLibraryConstantSignature("AGENT_BY_LEGACY_NAME").DocumentationString =
                        "This is used to find agents by legacy name";
                    GetLibraryConstantSignature("AGENT_BY_USERNAME").DocumentationString =
                        "This is used to find agents by username";
                    GetLibraryConstantSignature("ACTIVE").DocumentationString =
                        "Passed in llSensor library function to look for moving objects";
                    GetLibraryConstantSignature("PASSIVE").DocumentationString =
                        "Passed in llSensor library function to look for objects that aren't moving";
                    GetLibraryConstantSignature("SCRIPTED").DocumentationString =
                        "Passed in llSensor library function to look for scripted objects";
                    GetLibraryConstantSignature("CONTROL_FWD").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent forward control"
                            ;
                    GetLibraryConstantSignature("CONTROL_BACK").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent back control"
                            ;
                    GetLibraryConstantSignature("CONTROL_LEFT").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent left control"
                            ;
                    GetLibraryConstantSignature("CONTROL_RIGHT").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent right control"
                            ;
                    GetLibraryConstantSignature("CONTROL_ROT_LEFT").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent rotate left control"
                            ;
                    GetLibraryConstantSignature("CONTROL_ROT_RIGHT").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent rotate right control"
                            ;
                    GetLibraryConstantSignature("CONTROL_UP").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent up control"
                            ;
                    GetLibraryConstantSignature("CONTROL_DOWN").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent down control"
                            ;
                    GetLibraryConstantSignature("CONTROL_LBUTTON").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent left button control"
                            ;
                    GetLibraryConstantSignature("CONTROL_ML_LBUTTON").DocumentationString =
                        "Passed to llTakeControls library function and used control event handler to test for agent left button control with the agent in mouse look"
                            ;
                    GetLibraryConstantSignature("PERMISSION_DEBIT").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to take L$ from agent's account"
                            ;
                    GetLibraryConstantSignature("PERMISSION_TAKE_CONTROLS").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to take agent's controls"
                            ;
                    GetLibraryConstantSignature("PERMISSION_TRIGGER_ANIMATION").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to trigger animation on agent"
                            ;
                    GetLibraryConstantSignature("PERMISSION_ATTACH").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to attach/detach from agent"
                            ;
                    GetLibraryConstantSignature("PERMISSION_TRACK_CAMERA").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to track agent's camera"
                            ;
                    GetLibraryConstantSignature("PERMISSION_CONTROL_CAMERA").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to change agent's camera"
                            ;
                    GetLibraryConstantSignature("PERMISSION_TELEPORT").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to teleport an agent"
                            ;
                    GetLibraryConstantSignature("PERMISSION_SILENT_ESTATE_MANAGEMENT").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to silently modify estate access lists"
                            ;
                    GetLibraryConstantSignature("PERMISSION_OVERRIDE_ANIMATIONS").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to override animations on agent"
                            ;
                    GetLibraryConstantSignature("PERMISSION_RETURN_OBJECTS").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to return objects"
                            ;
                    GetLibraryConstantSignature("PERMISSION_CHANGE_LINKS").DocumentationString =
                        "Passed to llRequestPermissions library function to request permission to change an objects links"
                            ;

                    GetLibraryConstantSignature("DEBUG_CHANNEL").DocumentationString =
                        "Chat channel reserved for debug and error messages from scripts";
                    GetLibraryConstantSignature("PUBLIC_CHANNEL").DocumentationString =
                        "Chat channel that broadcasts to all nearby users";

                    GetLibraryConstantSignature("AGENT_FLYING").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is flying";
                    GetLibraryConstantSignature("AGENT_ATTACHMENTS").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent has attachments";
                    GetLibraryConstantSignature("AGENT_SCRIPTED").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent has scripted attachments";
                    GetLibraryConstantSignature("AGENT_SITTING").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is sitting";
                    GetLibraryConstantSignature("AGENT_ON_OBJECT").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is sitting on an object";
                    GetLibraryConstantSignature("AGENT_MOUSELOOK").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is in mouselook";
                    GetLibraryConstantSignature("AGENT_AWAY").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is in away mode";
                    GetLibraryConstantSignature("AGENT_WALKING").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is walking";
                    GetLibraryConstantSignature("AGENT_IN_AIR").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is in the air";
                    GetLibraryConstantSignature("AGENT_TYPING").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is typing";
                    GetLibraryConstantSignature("AGENT_CROUCHING").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is crouching";
                    GetLibraryConstantSignature("AGENT_BUSY").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is busy";
                    GetLibraryConstantSignature("AGENT_ALWAYS_RUN").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent has 'Always Run' enabled";
                    GetLibraryConstantSignature("AGENT_AUTOPILOT").DocumentationString =
                        "Returned by llGetAgentInfo if the Agent is under autopilot control";

                    GetLibraryConstantSignature("AGENT_LIST_PARCEL").DocumentationString =
                        "Passed to llGetAgentList to return only agents on the same parcel where the script is running"
                            ;
                    GetLibraryConstantSignature("AGENT_LIST_PARCEL_OWNER").DocumentationString =
                        "Passed to llGetAgentList to return only agents on any parcel in the region where the parcel owner is the same as the owner of the parcel under the scripted object"
                            ;
                    GetLibraryConstantSignature("AGENT_LIST_REGION").DocumentationString =
                        "Passed to llGetAgentList to return any/all agents in the region";



                    GetLibraryConstantSignature("OBJECT_UNKNOWN_DETAIL").DocumentationString =
                        "Returned by llGetObjectDetails when passed an invalid object parameter type";
                    GetLibraryConstantSignature("OBJECT_NAME").DocumentationString =
                        "Used with llGetObjectDetails to get an object's name";
                    GetLibraryConstantSignature("OBJECT_DESC").DocumentationString =
                        "Used with llGetObjectDetails to get an object's description";
                    GetLibraryConstantSignature("OBJECT_POS").DocumentationString =
                        "Used with llGetObjectDetails to get an object's position";
                    GetLibraryConstantSignature("OBJECT_ROT").DocumentationString =
                        "Used with llGetObjectDetails to get an object's rotation";
                    GetLibraryConstantSignature("OBJECT_VELOCITY").DocumentationString =
                        "Used with llGetObjectDetails to get an object's velocity";
                    GetLibraryConstantSignature("OBJECT_OWNER").DocumentationString =
                        "Used with llGetObjectDetails to get an object's owner's key.  Will be NULL_KEY if group owned"
                            ;
                    GetLibraryConstantSignature("OBJECT_GROUP").DocumentationString =
                        "Used with llGetObjectDetails to get an object's group's key";
                    GetLibraryConstantSignature("OBJECT_CREATOR").DocumentationString =
                        "Used with llGetObjectDetails to get an object's creator's key";
                    GetLibraryConstantSignature("OBJECT_RUNNING_SCRIPT_COUNT").DocumentationString =
                        "Gets the number of running scripts attached to the object or agent";
                    GetLibraryConstantSignature("OBJECT_TOTAL_SCRIPT_COUNT").DocumentationString =
                        "Gets the number of scripts, both running and stopped, attached to the object or agent."
                            ;
                    GetLibraryConstantSignature("OBJECT_SCRIPT_MEMORY").DocumentationString =
                        "Gets the total amount of script memory allocated to the object or agent, in bytes.".UnescapeXml
                            ();
                    GetLibraryConstantSignature("OBJECT_SCRIPT_TIME").DocumentationString =
                        "Gets the total amount of average script CPU time used by the object or agent, in seconds."
                            ;
                    GetLibraryConstantSignature("OBJECT_PRIM_EQUIVALENCE").DocumentationString =
                        "Gets the prim equivalence of the object.";
                    GetLibraryConstantSignature("OBJECT_PHYSICS").DocumentationString =
                        "Used with llGetObjectDetails to get an object's physics flag";
                    GetLibraryConstantSignature("OBJECT_PHANTOM").DocumentationString =
                        "Used with llGetObjectDetails to get an object's phantom flag";
                    GetLibraryConstantSignature("OBJECT_TEMP_ON_REZ").DocumentationString =
                        "Used with llGetObjectDetails to get an object's temporary flag";
                    GetLibraryConstantSignature("OBJECT_RENDER_WEIGHT").DocumentationString =
                        "Used with llGetObjectDetails to return an avatar's rendering weight";

                    GetLibraryConstantSignature("OBJECT_CHARACTER_TIME").DocumentationString =
                        "Used with llGetObjectDetails to get an object's average CPU time (in seconds) used by the object for navigation, if the object is a pathfinding character. Returns 0 for non-characters."
                            ;
                    GetLibraryConstantSignature("OBJECT_ROOT").DocumentationString =
                        "Used with llGetObjectDetails to get an object's root prim ID.";
                    GetLibraryConstantSignature("OBJECT_ATTACHED_POINT").DocumentationString =
                        "Used with llGetObjectDetails to get an object's attachment point.";

                    GetLibraryConstantSignature("OBJECT_RETURN_PARCEL").DocumentationString =
                        "Used with llReturnObjectsByOwner to return all objects on the same parcel as the script which are owned by 'owner'."
                            ;
                    GetLibraryConstantSignature("OBJECT_RETURN_PARCEL_OWNER").DocumentationString =
                        "Used with llReturnObjectsByOwner to return all objects owned by 'owner' which are over parcels owned by the owner of the script."
                            ;
                    GetLibraryConstantSignature("OBJECT_RETURN_REGION").DocumentationString =
                        "Used with llReturnObjectsByOwner to return all objects in the region owned by 'owner' - only works when the script is owned by the estate owner or an estate manager."
                            ;


                    GetLibraryConstantSignature("VEHICLE_REFERENCE_FRAME").DocumentationString =
                        "Rotation of vehicle axes relative to local frame";
                    GetLibraryConstantSignature("VEHICLE_LINEAR_FRICTION_TIMESCALE").DocumentationString =
                        "A vector of timescales for exponential decay of linear velocity along the three vehicle axes"
                            ;
                    GetLibraryConstantSignature("VEHICLE_ANGULAR_FRICTION_TIMESCALE").DocumentationString =
                        "A vector of timescales for exponential decay of angular velocity about the three vehicle axes"
                            ;
                    GetLibraryConstantSignature("VEHICLE_LINEAR_MOTOR_DIRECTION").DocumentationString =
                        "The linear velocity that the vehicle will try to achieve";
                    GetLibraryConstantSignature("VEHICLE_LINEAR_MOTOR_OFFSET").DocumentationString =
                        "An offset from the center of mass of the vehicle where the linear motor is applied".UnescapeXml
                            ();
                    GetLibraryConstantSignature("VEHICLE_ANGULAR_MOTOR_DIRECTION").DocumentationString =
                        "The angular velocity that the vehicle will try to achieve";
                    GetLibraryConstantSignature("VEHICLE_HOVER_HEIGHT").DocumentationString =
                        "The height the vehicle will try to hover";
                    GetLibraryConstantSignature("VEHICLE_HOVER_EFFICIENCY").DocumentationString =
                        "A slider between 0 (bouncy) and 1 (critically damped) hover behavior";
                    GetLibraryConstantSignature("VEHICLE_HOVER_TIMESCALE").DocumentationString =
                        "The period of time for the vehicle to achieve its hover height";
                    GetLibraryConstantSignature("VEHICLE_BUOYANCY").DocumentationString =
                        "A slider between 0 (no anti-gravity) and 1 (full anti-gravity)";
                    GetLibraryConstantSignature("VEHICLE_LINEAR_DEFLECTION_EFFICIENCY").DocumentationString =
                        "A slider between 0 (no deflection) and 1 (maximum strength)";
                    GetLibraryConstantSignature("VEHICLE_LINEAR_DEFLECTION_TIMESCALE").DocumentationString =
                        "The exponential timescale for the vehicle to redirect its velocity to be along its x-axis"
                            ;
                    GetLibraryConstantSignature("VEHICLE_LINEAR_MOTOR_TIMESCALE").DocumentationString =
                        "The exponential timescale for the vehicle to achive its full linear motor velocity".UnescapeXml
                            ();
                    GetLibraryConstantSignature("VEHICLE_LINEAR_MOTOR_DECAY_TIMESCALE").DocumentationString =
                        "The exponential timescale for the linear motor's effectiveness to decay toward zero"
                            ;
                    GetLibraryConstantSignature("VEHICLE_ANGULAR_DEFLECTION_EFFICIENCY").DocumentationString =
                        "A slider between 0 (no deflection) and 1 (maximum strength)";
                    GetLibraryConstantSignature("VEHICLE_ANGULAR_DEFLECTION_TIMESCALE").DocumentationString =
                        "The exponential timescale for the vehicle to achieve full angular deflection";
                    GetLibraryConstantSignature("VEHICLE_ANGULAR_MOTOR_TIMESCALE").DocumentationString =
                        "The exponential timescale for the vehicle to achive its full angular motor velocity"
                            ;
                    GetLibraryConstantSignature("VEHICLE_ANGULAR_MOTOR_DECAY_TIMESCALE").DocumentationString =
                        "The exponential timescale for the angular motor's effectiveness to decay toward zero"
                            ;
                    GetLibraryConstantSignature("VEHICLE_VERTICAL_ATTRACTION_EFFICIENCY").DocumentationString =
                        "A slider between 0 (bouncy) and 1 (critically damped) attraction of vehicle z-axis to world z-axis (vertical)"
                            ;
                    GetLibraryConstantSignature("VEHICLE_VERTICAL_ATTRACTION_TIMESCALE").DocumentationString =
                        "The exponential timescale for the vehicle to align its z-axis to the world z-axis (vertical)"
                            ;
                    GetLibraryConstantSignature("VEHICLE_BANKING_EFFICIENCY").DocumentationString =
                        "A slider between -1 (leans out of turns), 0 (no banking), and +1 (leans into turns)"
                            ;
                    GetLibraryConstantSignature("VEHICLE_BANKING_MIX").DocumentationString =
                        "A slider between 0 (static banking) and 1 (dynamic banking)";
                    GetLibraryConstantSignature("VEHICLE_BANKING_TIMESCALE").DocumentationString =
                        "The exponential timescale for the banking behavior to take full effect";
                    GetLibraryConstantSignature("VEHICLE_FLAG_NO_DEFLECTION_UP").DocumentationString =
                        "Prevents linear deflection along world-z axis";
                    GetLibraryConstantSignature("VEHICLE_FLAG_LIMIT_ROLL_ONLY").DocumentationString =
                        "Removes vertical attraction for changes in vehicle pitch";
                    GetLibraryConstantSignature("VEHICLE_FLAG_HOVER_WATER_ONLY").DocumentationString =
                        "Hover only pays attention to water level";
                    GetLibraryConstantSignature("VEHICLE_FLAG_HOVER_TERRAIN_ONLY").DocumentationString =
                        "Hover only pays attention to terrain height";
                    GetLibraryConstantSignature("VEHICLE_FLAG_HOVER_GLOBAL_HEIGHT").DocumentationString =
                        "Hover only pays attention to global height";
                    GetLibraryConstantSignature("VEHICLE_FLAG_HOVER_UP_ONLY").DocumentationString =
                        "Hover only pushes up";
                    GetLibraryConstantSignature("VEHICLE_FLAG_LIMIT_MOTOR_UP").DocumentationString =
                        "Prevents ground vehicles from motoring into the sky";
                    GetLibraryConstantSignature("VEHICLE_FLAG_MOUSELOOK_STEER").DocumentationString =
                        "Makes vehicle try to turn toward mouselook direction";
                    GetLibraryConstantSignature("VEHICLE_FLAG_MOUSELOOK_BANK").DocumentationString =
                        "Makes vehicle try to turn toward mouselook direction assuming banking is enabled";
                    GetLibraryConstantSignature("VEHICLE_FLAG_CAMERA_DECOUPLED").DocumentationString =
                        "Causes the camera look-at axis to NOT move when the vehicle rotates";

                    GetLibraryConstantSignature("CAMERA_PITCH").DocumentationString =
                        "(-45 to 80) (Adjusts the angular amount that the camera aims straight ahead vs. straight down, maintaining the same distance. Analogous to 'incidence'.)"
                            ;
                    GetLibraryConstantSignature("CAMERA_FOCUS_OFFSET").DocumentationString =
                        "(-10 to 10) A vector that adjusts the position of the camera focus position relative to the subject"
                            ;
                    GetLibraryConstantSignature("CAMERA_POSITION_LAG").DocumentationString =
                        "(0.0 to 3.0)  How much the camera lags as it tries to move towards its 'ideal' position"
                            ;
                    GetLibraryConstantSignature("CAMERA_FOCUS_LAG").DocumentationString =
                        "(0.0 to 3.0) How much the camera lags as it tries to aim towards the subject";
                    GetLibraryConstantSignature("CAMERA_DISTANCE").DocumentationString =
                        "(0.5 to 10) Sets how far away the camera wants to be from its subject";
                    GetLibraryConstantSignature("CAMERA_BEHINDNESS_ANGLE").DocumentationString =
                        "(0 to 180) Sets the angle in degrees within which the camera is not constrained by changes in subject rotation"
                            ;
                    GetLibraryConstantSignature("CAMERA_BEHINDNESS_LAG").DocumentationString =
                        "(0.0 to 3.0) Sets how strongly the camera is forced to stay behind the target if outside of behindness angle"
                            ;
                    GetLibraryConstantSignature("CAMERA_POSITION_THRESHOLD").DocumentationString =
                        "(0.0 to 4.0) Sets the radius of a sphere around the camera's ideal position within which it is not affected by subject motion"
                            ;
                    GetLibraryConstantSignature("CAMERA_FOCUS_THRESHOLD").DocumentationString =
                        "(0.0 to 4.0) Sets the radius of a sphere around the camera's subject position within which its focus is not affected by subject motion"
                            ;
                    GetLibraryConstantSignature("CAMERA_ACTIVE").DocumentationString =
                        "(0 or 1) Turns on or off scripted control of the camera";
                    GetLibraryConstantSignature("CAMERA_POSITION").DocumentationString =
                        "Sets the position of the camera";
                    GetLibraryConstantSignature("CAMERA_FOCUS").DocumentationString =
                        "Sets the focus (target position) of the camera";
                    GetLibraryConstantSignature("CAMERA_POSITION_LOCKED").DocumentationString =
                        "(0 or 1) Locks the camera position so it will not move";
                    GetLibraryConstantSignature("CAMERA_FOCUS_LOCKED").DocumentationString =
                        "(0 or 1) Locks the camera focus so it will not move";

                    GetLibraryConstantSignature("INVENTORY_TEXTURE").DocumentationString =
                        "Passed to task inventory library functions to reference textures";
                    GetLibraryConstantSignature("INVENTORY_SOUND").DocumentationString =
                        "Passed to task inventory library functions to reference sounds";
                    GetLibraryConstantSignature("INVENTORY_OBJECT").DocumentationString =
                        "Passed to task inventory library functions to reference objects";
                    GetLibraryConstantSignature("INVENTORY_SCRIPT").DocumentationString =
                        "Passed to task inventory library functions to reference scripts";
                    GetLibraryConstantSignature("INVENTORY_LANDMARK").DocumentationString =
                        "Passed to task inventory library functions to reference landmarks";
                    GetLibraryConstantSignature("INVENTORY_CLOTHING").DocumentationString =
                        "Passed to task inventory library functions to reference clothing";
                    GetLibraryConstantSignature("INVENTORY_NOTECARD").DocumentationString =
                        "Passed to task inventory library functions to reference notecards";
                    GetLibraryConstantSignature("INVENTORY_BODYPART").DocumentationString =
                        "Passed to task inventory library functions to reference body parts";
                    GetLibraryConstantSignature("INVENTORY_ANIMATION").DocumentationString =
                        "Passed to task inventory library functions to reference animations";
                    GetLibraryConstantSignature("INVENTORY_GESTURE").DocumentationString =
                        "Passed to task inventory library functions to reference gestures";
                    GetLibraryConstantSignature("INVENTORY_ALL").DocumentationString =
                        "Passed to task inventory library functions to reference all inventory items";
                    GetLibraryConstantSignature("INVENTORY_NONE").DocumentationString =
                        "Returned by llGetInventoryType when no item is found";

                    GetLibraryConstantSignature("ATTACH_CHEST").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to chest";
                    GetLibraryConstantSignature("ATTACH_HEAD").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to head";
                    GetLibraryConstantSignature("ATTACH_LSHOULDER").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left shoulder";
                    GetLibraryConstantSignature("ATTACH_RSHOULDER").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right shoulder";
                    GetLibraryConstantSignature("ATTACH_LHAND").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left hand";
                    GetLibraryConstantSignature("ATTACH_RHAND").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right hand";
                    GetLibraryConstantSignature("ATTACH_LFOOT").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left foot";
                    GetLibraryConstantSignature("ATTACH_RFOOT").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right foot";
                    GetLibraryConstantSignature("ATTACH_BACK").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to back";
                    GetLibraryConstantSignature("ATTACH_PELVIS").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to pelvis";
                    GetLibraryConstantSignature("ATTACH_MOUTH").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to mouth";
                    GetLibraryConstantSignature("ATTACH_CHIN").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to chin";
                    GetLibraryConstantSignature("ATTACH_LEAR").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left ear";
                    GetLibraryConstantSignature("ATTACH_REAR").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right ear";
                    GetLibraryConstantSignature("ATTACH_LEYE").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left eye";
                    GetLibraryConstantSignature("ATTACH_REYE").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right eye";
                    GetLibraryConstantSignature("ATTACH_NOSE").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to nose";
                    GetLibraryConstantSignature("ATTACH_RUARM").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right upper arm";
                    GetLibraryConstantSignature("ATTACH_RLARM").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right lower arm";
                    GetLibraryConstantSignature("ATTACH_LUARM").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left upper arm";
                    GetLibraryConstantSignature("ATTACH_LLARM").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left lower arm";
                    GetLibraryConstantSignature("ATTACH_RHIP").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right hip";
                    GetLibraryConstantSignature("ATTACH_RULEG").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right upper leg";
                    GetLibraryConstantSignature("ATTACH_RLLEG").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right lower leg";
                    GetLibraryConstantSignature("ATTACH_LHIP").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left hip";
                    GetLibraryConstantSignature("ATTACH_LULEG").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left upper leg";
                    GetLibraryConstantSignature("ATTACH_LLLEG").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left lower leg";
                    GetLibraryConstantSignature("ATTACH_BELLY").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to belly";
                    GetLibraryConstantSignature("ATTACH_LEFT_PEC").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to left pectoral";
                    GetLibraryConstantSignature("ATTACH_RIGHT_PEC").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to right pectoral";
                    GetLibraryConstantSignature("ATTACH_NECK").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to neck";
                    GetLibraryConstantSignature("ATTACH_AVATAR_CENTER").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to avatar center";
                    GetLibraryConstantSignature("ATTACH_BRIDGE").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to bridge";
                    GetLibraryConstantSignature("ATTACH_HUD_BOTTOM").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to bottom hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_BOTTOM_LEFT").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to bottom left hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_BOTTOM_RIGHT").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to bottom right hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_CENTER_1").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to center 1 hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_CENTER_2").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to center 2 hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_TOP_CENTER").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to top center hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_TOP_LEFT").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to top left hud area";
                    GetLibraryConstantSignature("ATTACH_HUD_TOP_RIGHT").DocumentationString =
                        "Passed to llAttachToAvatar to attach task to top right hud area";

                    GetLibraryConstantSignature("LAND_LEVEL").DocumentationString =
                        "Passed to llModifyLand to level terrain";
                    GetLibraryConstantSignature("LAND_RAISE").DocumentationString =
                        "Passed to llModifyLand to raise terrain";
                    GetLibraryConstantSignature("LAND_LOWER").DocumentationString =
                        "Passed to llModifyLand to lower terrain";
                    GetLibraryConstantSignature("LAND_SMOOTH").DocumentationString =
                        "Passed to llModifyLand to smooth terrain";
                    GetLibraryConstantSignature("LAND_NOISE").DocumentationString =
                        "Passed to llModifyLand to randomize terrain";
                    GetLibraryConstantSignature("LAND_REVERT").DocumentationString =
                        "Passed to llModifyLand to revert terrain toward original state";
                    GetLibraryConstantSignature("LAND_SMALL_BRUSH").DocumentationString =
                        "Passed to llModifyLand to modify small land areas";
                    GetLibraryConstantSignature("LAND_MEDIUM_BRUSH").DocumentationString =
                        "Passed to llModifyLand to modify medium land areas";
                    GetLibraryConstantSignature("LAND_LARGE_BRUSH").DocumentationString =
                        "Passed to llModifyLand to modify large land areas";

                    GetLibraryConstantSignature("DATA_PAYINFO").DocumentationString =
                        "Passed to llRequestAgentData to get payment status of an agent";
                    GetLibraryConstantSignature("DATA_ONLINE").DocumentationString =
                        "Passed to llRequestAgentData to determine if agent is online";
                    GetLibraryConstantSignature("DATA_NAME").DocumentationString =
                        "Passed to llRequestAgentData to get full agent name";
                    GetLibraryConstantSignature("DATA_BORN").DocumentationString =
                        "Passed to llRequestAgentData to get born on date as a string";
                    GetLibraryConstantSignature("DATA_RATING").DocumentationString =
                        "Passed to llRequestAgentData to get a comma separated sting of integer ratings";
                    GetLibraryConstantSignature("DATA_SIM_POS").DocumentationString =
                        "Passed to llRequestSimulatorData to get a string (cast to vector) of a simulator's global position"
                            ;
                    GetLibraryConstantSignature("DATA_SIM_STATUS").DocumentationString =
                        "Passed to llRequestSimulatorData to get the status of a simulator";
                    GetLibraryConstantSignature("DATA_SIM_RATING").DocumentationString =
                        "Passed to llRequestSimulatorData to get the rating of a simulator";

                    GetLibraryConstantSignature("PAYMENT_INFO_ON_FILE").DocumentationString =
                        "Used with llRequestAgentData to tell if Agent is of &quot;Payment Info On File&quot; status"
                            ;
                    GetLibraryConstantSignature("PAYMENT_INFO_USED").DocumentationString =
                        "Used with llRequestAgentData to tell if Agent is of &quot;Payment Info Used&quot; status"
                            ;

                    GetLibraryConstantSignature("ANIM_ON").DocumentationString =
                        "Enable texture animation";
                    GetLibraryConstantSignature("LOOP").DocumentationString =
                        "Loop when animating textures";
                    GetLibraryConstantSignature("REVERSE").DocumentationString =
                        "Animate in the reverse direction";
                    GetLibraryConstantSignature("PING_PONG").DocumentationString =
                        "Animate forward, then reverse";
                    GetLibraryConstantSignature("SMOOTH").DocumentationString =
                        "Textures slides, instead of stepping";
                    GetLibraryConstantSignature("ROTATE").DocumentationString =
                        "Rotates the texture, instead of using frames";
                    GetLibraryConstantSignature("SCALE").DocumentationString =
                        "Scales the texture, instead of using frames";

                    GetLibraryConstantSignature("ALL_SIDES").DocumentationString =
                        "Passed to various texture and color library functions to modify all sides";

                    GetLibraryConstantSignature("LINK_SET").DocumentationString =
                        "Passed to various link functions to modify all blocks in the object";
                    GetLibraryConstantSignature("LINK_ROOT").DocumentationString =
                        "Passed to various link functions to modify only the root block (no effect on single block objects)"
                            ;
                    GetLibraryConstantSignature("LINK_ALL_OTHERS").DocumentationString =
                        "Passed to various link functions to modify all other blocks in the object";
                    GetLibraryConstantSignature("LINK_ALL_CHILDREN").DocumentationString =
                        "Passed to various link functions to modify all child blocks in the object";
                    GetLibraryConstantSignature("LINK_THIS").DocumentationString =
                        "Passed to various link functions to modify only the calling block";

                    GetLibraryConstantSignature("CHANGED_INVENTORY").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's inventory";
                    GetLibraryConstantSignature("CHANGED_COLOR").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's color";
                    GetLibraryConstantSignature("CHANGED_SHAPE").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's shape parameters"
                            ;
                    GetLibraryConstantSignature("CHANGED_SCALE").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's scale";
                    GetLibraryConstantSignature("CHANGED_TEXTURE").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's texture";
                    GetLibraryConstantSignature("CHANGED_LINK").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's link status";
                    GetLibraryConstantSignature("CHANGED_ALLOWED_DROP").DocumentationString =
                        "Parameter of changed event handler used to indicate a user dropped an inventory item;onto task that was allowed only by llAllowInventoryDrop function call"
                            ;
                    GetLibraryConstantSignature("CHANGED_OWNER").DocumentationString =
                        "Parameter of changed event handler used to indicate change to task's owner ONLY when an object is sold as original or deeded to group"
                            ;
                    GetLibraryConstantSignature("CHANGED_REGION").DocumentationString =
                        "Parameter of changed event handler used to indicate the region has changed";
                    GetLibraryConstantSignature("CHANGED_TELEPORT").DocumentationString =
                        "Parameter of changed event handler used to indicate teleport has completed";
                    GetLibraryConstantSignature("CHANGED_REGION_START").DocumentationString =
                        "Parameter of changed event handler used to indicate the region has been restarted";
                    GetLibraryConstantSignature("CHANGED_MEDIA").DocumentationString =
                        "Parameter of changed event handler used to indicate that media has changed on a face of the task"
                            ;

                    GetLibraryConstantSignature("TYPE_INTEGER").DocumentationString =
                        "Indicates that the list entry is holding an integer";
                    GetLibraryConstantSignature("TYPE_FLOAT").DocumentationString =
                        "Indicates that the list entry is holding an float";
                    GetLibraryConstantSignature("TYPE_STRING").DocumentationString =
                        "Indicates that the list entry is holding an string";
                    GetLibraryConstantSignature("TYPE_KEY").DocumentationString =
                        "Indicates that the list entry is holding an key";
                    GetLibraryConstantSignature("TYPE_VECTOR").DocumentationString =
                        "Indicates that the list entry is holding an vector";
                    GetLibraryConstantSignature("TYPE_ROTATION").DocumentationString =
                        "Indicates that the list entry is holding an rotation";
                    GetLibraryConstantSignature("TYPE_INVALID").DocumentationString =
                        "Indicates that this wasn't a valid list entry";

                    GetLibraryConstantSignature("REMOTE_DATA_CHANNEL").DocumentationString =
                        "Value of event_type in remote_event after successful llOpenRemoteDataChannel";
                    GetLibraryConstantSignature("REMOTE_DATA_REQUEST").DocumentationString =
                        "Value of event_type in remote_event if XML-RPC request is received";
                    GetLibraryConstantSignature("REMOTE_DATA_REPLY").DocumentationString =
                        "Value of event_type in remote_event if XML-RPC reply is received";

                    GetLibraryConstantSignature("PRIM_TYPE").DocumentationString =
                        "Followed by PRIM_TYPE_BOX, PRIM_TYPE_CYLINDER, PRIM_TYPE_PRISM, PRIM_TYPE_SPHERE, PRIM_TYPE_TORUS, PRIM_TYPE_TUBE, or PRIM_TYPE_SCULPT and their arguments"
                            ;
                    GetLibraryConstantSignature("PRIM_MATERIAL").DocumentationString =
                        "Followed by PRIM_MATERIAL_STONE, PRIM_MATERIAL_METAL, PRIM_MATERIAL_GLASS, PRIM_MATERIAL_WOOD, PRIM_MATERIAL_FLESH, PRIM_MATERIAL_PLASTIC, or PRIM_MATERIAL_RUBBER"
                            ;
                    GetLibraryConstantSignature("PRIM_PHYSICS").DocumentationString =
                        "Sets physics to TRUE or FALSE";
                    GetLibraryConstantSignature("PRIM_FLEXIBLE").DocumentationString =
                        "Followed by TRUE or FALSE, integer softness, float gravity, float friction, float wind, float tension, and vector force"
                            ;
                    GetLibraryConstantSignature("PRIM_POINT_LIGHT").DocumentationString =
                        "Followed by TRUE or FALSE, vector color, float intensity, float radius, float falloff"
                            ;
                    GetLibraryConstantSignature("PRIM_TEMP_ON_REZ").DocumentationString =
                        "Sets temporary on rez to TRUE or FALSE";
                    GetLibraryConstantSignature("PRIM_PHANTOM").DocumentationString =
                        "Sets phantom to TRUE or FALSE";
                    GetLibraryConstantSignature("PRIM_CAST_SHADOWS").DocumentationString =
                        "DEPRECATED. Takes 1 parameter, an integer, but has no effect when set and always returns 0 if used in llGetPrimitiveParams"
                            ;
                    GetLibraryConstantSignature("PRIM_POSITION").DocumentationString =
                        "Sets primitive position to a vector position";
                    GetLibraryConstantSignature("PRIM_SIZE").DocumentationString =
                        "Sets primitive size to a vector size";
                    GetLibraryConstantSignature("PRIM_ROTATION").DocumentationString =
                        "Sets primitive rotation";
                    GetLibraryConstantSignature("PRIM_TEXT").DocumentationString =
                        "Used to get or set the object's floating text.";
                    GetLibraryConstantSignature("PRIM_TEXTURE").DocumentationString =
                        "Followed by an integer face, key id, vector repeats, vector offsets,;and float rotation in radians"
                            ;
                    GetLibraryConstantSignature("PRIM_COLOR").DocumentationString =
                        "Followed by an integer face, vector color, and float alpha";
                    GetLibraryConstantSignature("PRIM_BUMP_SHINY").DocumentationString =
                        "Followed by an integer face, one of PRIM_SHINY_NONE, PRIM_SHINY_LOW,;PRIM_SHINY_MEDIUM, or PRIM_SHINY_HIGH,;and one of PRIM_BUMP_NONE, PRIM_BUMP_BRIGHT, PRIM_BUMP_DARK, etc"
                            ;
                    GetLibraryConstantSignature("PRIM_FULLBRIGHT").DocumentationString =
                        "Followed by an integer face, and TRUE or FALSE";
                    GetLibraryConstantSignature("PRIM_TEXGEN").DocumentationString =
                        "Followed by an integer face, and one of PRIM_TEXGEN_DEFAULT or PRIM_TEXGEN_PLANAR";
                    GetLibraryConstantSignature("PRIM_GLOW").DocumentationString =
                        "Followed by an integer face, and a float from 0.0 to 1.0 specifying glow amount";
                    GetLibraryConstantSignature("PRIM_POS_LOCAL").DocumentationString =
                        "Sets the prim's local position.";
                    GetLibraryConstantSignature("PRIM_ROT_LOCAL").DocumentationString =
                        "Sets the prim's local rotation.";
                    GetLibraryConstantSignature("PRIM_NAME").DocumentationString = "Sets the prim's name.";
                    GetLibraryConstantSignature("PRIM_DESC").DocumentationString =
                        "Sets the prim's description.";
                    GetLibraryConstantSignature("PRIM_OMEGA").DocumentationString =
                        "Makes the object spin at the specified axis and rate.";
                    GetLibraryConstantSignature("PRIM_LINK_TARGET").DocumentationString =
                        "Used to get or set multiple links with a single PrimParameters call.";
                    GetLibraryConstantSignature("PRIM_SLICE").DocumentationString =
                        "Get and set the &quot;slice&quot; parameter of all shapes. Takes a vector parameter of the form &lt;start_slice, end_slice, 0&gt;"
                            ;

                    GetLibraryConstantSignature("PRIM_TYPE_BOX").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;vector top size, and vector top shear"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_CYLINDER").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;vector top size, and vector top shear"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_PRISM").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;vector top size, and vector top shear"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_SPHERE").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;and vector dimple"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_TORUS").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;vector hole size, vector top shear, vector advanced cut, vector taper,;float revolutions, float radius offset, and float skew"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_TUBE").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;vector hole size, vector top shear, vector advanced cut, vector taper,;float revolutions, float radius offset, and float skew"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_RING").DocumentationString =
                        "Followed by integer hole shape, vector cut, float hollow, vector twist,;vector hole size, vector top shear, vector advanced cut, vector taper,;float revolutions, float radius offset, and float skew"
                            ;
                    GetLibraryConstantSignature("PRIM_TYPE_SCULPT").DocumentationString =
                        "Followed by a key/string texture uuid, and one of PRIM_SCULPT_TYPE_SPHERE, PRIM_SCULPT_TYPE_TORUS, PRIM_SCULPT_TYPE_PLANE, or PRIM_SCULPT_TYPE_CYLINDER"
                            ;

                    GetLibraryConstantSignature("PRIM_HOLE_DEFAULT").DocumentationString =
                        "Sets hole type to match the prim type";
                    GetLibraryConstantSignature("PRIM_HOLE_SQUARE").DocumentationString =
                        "Sets hole type to square";
                    GetLibraryConstantSignature("PRIM_HOLE_CIRCLE").DocumentationString =
                        "Sets hole type to circle";
                    GetLibraryConstantSignature("PRIM_HOLE_TRIANGLE").DocumentationString =
                        "Sets hole type to triangle";

                    GetLibraryConstantSignature("PRIM_MATERIAL_STONE").DocumentationString =
                        "Sets material to stone";
                    GetLibraryConstantSignature("PRIM_MATERIAL_METAL").DocumentationString =
                        "Sets material to metal";
                    GetLibraryConstantSignature("PRIM_MATERIAL_GLASS").DocumentationString =
                        "Sets material to glass";
                    GetLibraryConstantSignature("PRIM_MATERIAL_WOOD").DocumentationString =
                        "Sets material to wood";
                    GetLibraryConstantSignature("PRIM_MATERIAL_FLESH").DocumentationString =
                        "Sets material to flesh";
                    GetLibraryConstantSignature("PRIM_MATERIAL_PLASTIC").DocumentationString =
                        "Sets material to plastic";
                    GetLibraryConstantSignature("PRIM_MATERIAL_RUBBER").DocumentationString =
                        "Sets material to rubber";
                    GetLibraryConstantSignature("PRIM_MATERIAL_LIGHT").DocumentationString =
                        "Sets material to light";

                    GetLibraryConstantSignature("PRIM_SHINY_NONE").DocumentationString = "No shininess";
                    GetLibraryConstantSignature("PRIM_SHINY_LOW").DocumentationString = "Low shininess";
                    GetLibraryConstantSignature("PRIM_SHINY_MEDIUM").DocumentationString =
                        "Medium shininess";
                    GetLibraryConstantSignature("PRIM_SHINY_HIGH").DocumentationString = "High shininess";

                    GetLibraryConstantSignature("PRIM_BUMP_NONE").DocumentationString = "No bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_BRIGHT").DocumentationString =
                        "Generate bump map from highlights";
                    GetLibraryConstantSignature("PRIM_BUMP_DARK").DocumentationString =
                        "Generate bump map from lowlights";
                    GetLibraryConstantSignature("PRIM_BUMP_WOOD").DocumentationString = "Wood bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_BARK").DocumentationString = "Bark bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_BRICKS").DocumentationString = "Brick bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_CHECKER").DocumentationString =
                        "Checker bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_CONCRETE").DocumentationString =
                        "Concrete bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_TILE").DocumentationString = "Tile bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_STONE").DocumentationString = "Stone bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_DISKS").DocumentationString = "Disk bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_GRAVEL").DocumentationString =
                        "Gravel bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_BLOBS").DocumentationString = "Blob bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_SIDING").DocumentationString =
                        "Siding bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_LARGETILE").DocumentationString =
                        "Large tile bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_STUCCO").DocumentationString =
                        "Stucco bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_SUCTION").DocumentationString =
                        "Suction cup bump map";
                    GetLibraryConstantSignature("PRIM_BUMP_WEAVE").DocumentationString = "Weave bump map";

                    GetLibraryConstantSignature("PRIM_TEXGEN_DEFAULT").DocumentationString =
                        "Default texture mapping";
                    GetLibraryConstantSignature("PRIM_TEXGEN_PLANAR").DocumentationString =
                        "Planar texture mapping";

                    GetLibraryConstantSignature("PRIM_SCULPT_TYPE_SPHERE").DocumentationString =
                        "Stitch edges in a sphere-like way";
                    GetLibraryConstantSignature("PRIM_SCULPT_TYPE_TORUS").DocumentationString =
                        "Stitch edges in a torus-like way";
                    GetLibraryConstantSignature("PRIM_SCULPT_TYPE_PLANE").DocumentationString =
                        "Do not stitch edges";
                    GetLibraryConstantSignature("PRIM_SCULPT_TYPE_CYLINDER").DocumentationString =
                        "Stitch edges in a cylinder-like way";
                    GetLibraryConstantSignature("PRIM_SCULPT_TYPE_MASK").DocumentationString =
                        "Mask used to determine stitching type";
                    GetLibraryConstantSignature("PRIM_SCULPT_FLAG_INVERT").DocumentationString =
                        "Flag to specify that the surface normals should be inverted";
                    GetLibraryConstantSignature("PRIM_SCULPT_FLAG_MIRROR").DocumentationString =
                        "Flag to specify that the prim should be reflected along X axis";

                    GetLibraryConstantSignature("MASK_BASE").DocumentationString = "Base permissions";
                    GetLibraryConstantSignature("MASK_OWNER").DocumentationString = "Owner permissions";
                    GetLibraryConstantSignature("MASK_GROUP").DocumentationString = "Group permissions";
                    GetLibraryConstantSignature("MASK_EVERYONE").DocumentationString =
                        "Everyone permissions";
                    GetLibraryConstantSignature("MASK_NEXT").DocumentationString =
                        "Next owner permissions";

                    GetLibraryConstantSignature("PERM_TRANSFER").DocumentationString =
                        "Transfer permission";
                    GetLibraryConstantSignature("PERM_MODIFY").DocumentationString = "Modify permission";
                    GetLibraryConstantSignature("PERM_COPY").DocumentationString = "Copy permission";
                    GetLibraryConstantSignature("PERM_MOVE").DocumentationString = "Move permission";
                    GetLibraryConstantSignature("PERM_ALL").DocumentationString =
                        "Move/Modify/Copy/Transfer permissions";

                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_STOP").DocumentationString =
                        "Stop media stream";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_PAUSE").DocumentationString =
                        "Pause media stream";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_PLAY").DocumentationString =
                        "Play media stream";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_LOOP").DocumentationString =
                        "Loop media stream";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_LOOP_SET").DocumentationString =
                        "Used to get or set the parcel's media loop duration.";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_TEXTURE").DocumentationString =
                        "Get or set the parcel's media texture";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_URL").DocumentationString =
                        "Get or set the parcel's media url";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_TYPE").DocumentationString =
                        "Get or set the parcel's media mimetype";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_DESC").DocumentationString =
                        "Get or set the parcel's media description";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_TIME").DocumentationString =
                        "Set media stream to specific time";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_SIZE").DocumentationString =
                        "Get or set the parcel's media pixel resolution";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_AGENT").DocumentationString =
                        "Allows media stream commands to apply to only one agent";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_UNLOAD").DocumentationString =
                        "Unloads the media stream";
                    GetLibraryConstantSignature("PARCEL_MEDIA_COMMAND_AUTO_ALIGN").DocumentationString =
                        "Auto aligns the media stream to the texture size.  May cause a performance hit and loss of some visual quality"
                            ;

                    GetLibraryConstantSignature("PAY_HIDE").DocumentationString =
                        "Used with llSetPayPrice to hide a button";
                    GetLibraryConstantSignature("PAY_DEFAULT").DocumentationString =
                        "Used with llSetPayPrice to use the default price for a button";

                    GetLibraryConstantSignature("LIST_STAT_MAX").DocumentationString =
                        "Used with llListStatistics to find the largest number in a list";
                    GetLibraryConstantSignature("LIST_STAT_MIN").DocumentationString =
                        "Used with llListStatistics to find the smallest number in a list";
                    GetLibraryConstantSignature("LIST_STAT_MEAN").DocumentationString =
                        "Used with llListStatistics to find the mean of the numbers in a list";
                    GetLibraryConstantSignature("LIST_STAT_MEDIAN").DocumentationString =
                        "Used with llListStatistics to find the median of the numbers in a list";
                    GetLibraryConstantSignature("LIST_STAT_STD_DEV").DocumentationString =
                        "Used with llListStatistics to find the standard deviation of the numbers in a list".UnescapeXml
                            ();
                    GetLibraryConstantSignature("LIST_STAT_SUM").DocumentationString =
                        "Used with llListStatistics to find the sum of the numbers in a list";
                    GetLibraryConstantSignature("LIST_STAT_SUM_SQUARES").DocumentationString =
                        "Used with llListStatistics to find the sum of the squares of the numbers in a list".UnescapeXml
                            ();
                    GetLibraryConstantSignature("LIST_STAT_NUM_COUNT").DocumentationString =
                        "Used with llListStatistics to find how many numbers are in a list";
                    GetLibraryConstantSignature("LIST_STAT_GEOMETRIC_MEAN").DocumentationString =
                        "Used with llListStatistics to find the geometric mean of the numbers in a list (all numbers must be > 0)"
                            ;
                    GetLibraryConstantSignature("LIST_STAT_RANGE").DocumentationString =
                        "Used with llListStatistics to find the range of the numbers in a list";

                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_FLY").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows flying";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_GROUP_SCRIPTS").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows group scripts";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_SCRIPTS").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows outside scripts";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_LANDMARK").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows landmarks to be created";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_TERRAFORM").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows anyone to terraform the land".UnescapeXml
                            ();
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_DAMAGE").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows damage";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_CREATE_OBJECTS").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows anyone to create objects";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_CREATE_GROUP_OBJECTS").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows group members or objects to create objects"
                            ;
                    GetLibraryConstantSignature("PARCEL_FLAG_USE_ACCESS_GROUP").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel limits access to a group";
                    GetLibraryConstantSignature("PARCEL_FLAG_USE_ACCESS_LIST").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel limits access to a list of residents"
                            ;
                    GetLibraryConstantSignature("PARCEL_FLAG_USE_BAN_LIST").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel uses a ban list";
                    GetLibraryConstantSignature("PARCEL_FLAG_USE_LAND_PASS_LIST").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows passes to be purchased";
                    GetLibraryConstantSignature("PARCEL_FLAG_LOCAL_SOUND_ONLY").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel restricts spacialized sound to the parcel"
                            ;
                    GetLibraryConstantSignature("PARCEL_FLAG_RESTRICT_PUSHOBJECT").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel restricts llPushObject() calls";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_ALL_OBJECT_ENTRY").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel allows all objects to enter";
                    GetLibraryConstantSignature("PARCEL_FLAG_ALLOW_GROUP_OBJECT_ENTRY").DocumentationString =
                        "Used with llGetParcelFlags to find if a parcel only allows group (and owner) objects to enter"
                            ;

                    GetLibraryConstantSignature("REGION_FLAG_ALLOW_DAMAGE").DocumentationString =
                        "Used with llGetRegionFlags to find if a region is entirely damage enabled";
                    GetLibraryConstantSignature("REGION_FLAG_FIXED_SUN").DocumentationString =
                        "Used with llGetRegionFlags to find if a region has a fixed sun position";
                    GetLibraryConstantSignature("REGION_FLAG_BLOCK_TERRAFORM").DocumentationString =
                        "Used with llGetRegionFlags to find if a region terraforming disabled";
                    GetLibraryConstantSignature("REGION_FLAG_SANDBOX").DocumentationString =
                        "Used with llGetRegionFlags to find if a region is a sandbox";
                    GetLibraryConstantSignature("REGION_FLAG_DISABLE_COLLISIONS").DocumentationString =
                        "Used with llGetRegionFlags to find if a region has disabled collisions";
                    GetLibraryConstantSignature("REGION_FLAG_DISABLE_PHYSICS").DocumentationString =
                        "Used with llGetRegionFlags to find if a region has disabled physics";
                    GetLibraryConstantSignature("REGION_FLAG_BLOCK_FLY").DocumentationString =
                        "Used with llGetRegionFlags to find if a region blocks flying";
                    GetLibraryConstantSignature("REGION_FLAG_ALLOW_DIRECT_TELEPORT").DocumentationString =
                        "Used with llGetRegionFlags to find if a region allows direct teleports";
                    GetLibraryConstantSignature("REGION_FLAG_RESTRICT_PUSHOBJECT").DocumentationString =
                        "Used with llGetRegionFlags to find if a region restricts llPushObject() calls";

                    GetLibraryConstantSignature("HTTP_METHOD").DocumentationString =
                        "Used with llHTTPRequest to specify the method, such as 'GET' or 'POST'";
                    GetLibraryConstantSignature("HTTP_MIMETYPE").DocumentationString =
                        "Used with llHTTPRequest to specify the MIME type, defaults to 'text/plain'";
                    GetLibraryConstantSignature("HTTP_VERIFY_CERT").DocumentationString =
                        "Used with llHTTPRequest to specify SSL certificate verification";
                    GetLibraryConstantSignature("HTTP_BODY_TRUNCATED").DocumentationString =
                        "Used with http_response to indicate truncation point in bytes";
                    GetLibraryConstantSignature("HTTP_BODY_MAXLENGTH").DocumentationString =
                        "Used with llHTTPRequest to specify the maximum body size for the date returned from the request. Mono scripts can request from 1byte to 16k, non-mono scripts can request from 1byte to 4k. The default is 2k."
                            ;
                    GetLibraryConstantSignature("HTTP_VERBOSE_THROTTLE").DocumentationString =
                        "The integer constant HTTP_VERBOSE_THROTTLE has the value 4";
                    GetLibraryConstantSignature("HTTP_CUSTOM_HEADER").DocumentationString =
                        "The integer constant HTTP_CUSTOM_HEADER has the value 5";
                    GetLibraryConstantSignature("HTTP_PRAGMA_NO_CACHE").DocumentationString =
                        "The integer constant HTTP_PRAGMA_NO_CACHE has the value 6";

                    GetLibraryConstantSignature("PARCEL_COUNT_TOTAL").DocumentationString =
                        "Used with llGetParcelPrimCount to get the total number of prims on the parcel";
                    GetLibraryConstantSignature("PARCEL_COUNT_OWNER").DocumentationString =
                        "Used with llGetParcelPrimCount to get the number of prims on the parcel owned by the owner"
                            ;
                    GetLibraryConstantSignature("PARCEL_COUNT_GROUP").DocumentationString =
                        "Used with llGetParcelPrimCount to get the number of prims on the parcel owned by the group"
                            ;
                    GetLibraryConstantSignature("PARCEL_COUNT_OTHER").DocumentationString =
                        "Used with llGetParcelPrimCount to get the number of prims on the parcel owned by others"
                            ;
                    GetLibraryConstantSignature("PARCEL_COUNT_SELECTED").DocumentationString =
                        "Used with llGetParcelPrimCount to get the number of prims on the parcel currently selected or sat upon"
                            ;
                    GetLibraryConstantSignature("PARCEL_COUNT_TEMP").DocumentationString =
                        "Used with llGetParcelPrimCount to get the number of prims on the parcel that are temp on rez"
                            ;

                    GetLibraryConstantSignature("PARCEL_DETAILS_NAME").DocumentationString =
                        "Used with llGetParcelDetails to get the parcel name";
                    GetLibraryConstantSignature("PARCEL_DETAILS_DESC").DocumentationString =
                        "Used with llGetParcelDetails to get the parcel description";
                    GetLibraryConstantSignature("PARCEL_DETAILS_OWNER").DocumentationString =
                        "Used with llGetParcelDetails to get the parcel owner id";
                    GetLibraryConstantSignature("PARCEL_DETAILS_GROUP").DocumentationString =
                        "Used with llGetParcelDetails to get the parcel group id";
                    GetLibraryConstantSignature("PARCEL_DETAILS_AREA").DocumentationString =
                        "Used with llGetParcelDetails to get the parcel area in square meters";
                    GetLibraryConstantSignature("PARCEL_DETAILS_ID").DocumentationString =
                        "Used with llGetParcelDetails to get the parcel id";
                    GetLibraryConstantSignature("PARCEL_DETAILS_SEE_AVATARS").DocumentationString =
                        "Used with llGetParcelDetails to get the avatars visibility setting";

                    GetLibraryConstantSignature("STRING_TRIM_HEAD").DocumentationString =
                        "Used with llStringTrim to trim leading spaces from a string";
                    GetLibraryConstantSignature("STRING_TRIM_TAIL").DocumentationString =
                        "Used with llStringTrim to trim trailing spaces from a string";
                    GetLibraryConstantSignature("STRING_TRIM").DocumentationString =
                        "Used with llStringTrim to trim both leading and trailing spaces from a string";

                    GetLibraryConstantSignature("CLICK_ACTION_NONE").DocumentationString =
                        "Used with llSetClickAction to disable the click action";
                    GetLibraryConstantSignature("CLICK_ACTION_TOUCH").DocumentationString =
                        "Used with llSetClickAction to set touch as the default action when object is clicked"
                            ;
                    GetLibraryConstantSignature("CLICK_ACTION_SIT").DocumentationString =
                        "Used with llSetClickAction to set sit as the default action when object is clicked".UnescapeXml
                            ();
                    GetLibraryConstantSignature("CLICK_ACTION_BUY").DocumentationString =
                        "Used with llSetClickAction to set buy as the default action when object is clicked".UnescapeXml
                            ();
                    GetLibraryConstantSignature("CLICK_ACTION_PAY").DocumentationString =
                        "Used with llSetClickAction to set pay as the default action when object is clicked".UnescapeXml
                            ();
                    GetLibraryConstantSignature("CLICK_ACTION_OPEN").DocumentationString =
                        "Used with llSetClickAction to set open as the default action when object is clicked"
                            ;
                    GetLibraryConstantSignature("CLICK_ACTION_PLAY").DocumentationString =
                        "Used with llSetClickAction to set play as the default action when object is clicked"
                            ;
                    GetLibraryConstantSignature("CLICK_ACTION_OPEN_MEDIA").DocumentationString =
                        "Used with llSetClickAction to set open-media as the default action when object is clicked"
                            ;
                    GetLibraryConstantSignature("CLICK_ACTION_ZOOM").DocumentationString =
                        "Used with llSetClickAction to set zoom in as the default action when object is clicked"
                            ;

                    GetLibraryConstantSignature("TOUCH_INVALID_TEXCOORD").DocumentationString =
                        "Value returned by llDetectedTouchUV() and llDetectedTouchST() when the touch position is not valid"
                            ;
                    GetLibraryConstantSignature("TOUCH_INVALID_VECTOR").DocumentationString =
                        "Value returned by llDetectedTouchPos(), llDetectedTouchNormal(), and llDetectedTouchBinormal() when the touch position is not valid"
                            ;
                    GetLibraryConstantSignature("TOUCH_INVALID_FACE").DocumentationString =
                        "Value returned by llDetectedTouchFace() when the touch position is not valid";

                    GetLibraryConstantSignature("PRIM_MEDIA_ALT_IMAGE_ENABLE").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to enable the default alt image for media";
                    GetLibraryConstantSignature("PRIM_MEDIA_CONTROLS").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine the controls shown for media";
                    GetLibraryConstantSignature("PRIM_MEDIA_CURRENT_URL").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to navigate/access the current URL";
                    GetLibraryConstantSignature("PRIM_MEDIA_HOME_URL").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to access the home URL";
                    GetLibraryConstantSignature("PRIM_MEDIA_AUTO_LOOP").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine if media should auto-loop (if applicable)"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_AUTO_PLAY").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine if media should start playing as soon as it is created"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_AUTO_SCALE").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine if media should scale to fit the face it is on"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_AUTO_ZOOM").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine if the user would zoom in when viewing media"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_FIRST_CLICK_INTERACT").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine whether the user interacts with media or not when she first clicks it (versus selection)"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_WIDTH_PIXELS").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to access the media's width in pixels";
                    GetLibraryConstantSignature("PRIM_MEDIA_HEIGHT_PIXELS").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to access the media's height in pixels";
                    GetLibraryConstantSignature("PRIM_MEDIA_WHITELIST_ENABLE").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine if the domain whitelist is enabled"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_WHITELIST").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to access the media's list of allowable URL prefixes to navigate to"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_PERMS_INTERACT").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine the permissions for who can interact with the media"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_PERMS_CONTROL").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams to determine the permissions for who has controls"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_PARAM_MAX").DocumentationString =
                        "The value of the largest media param";

                    GetLibraryConstantSignature("PRIM_MEDIA_CONTROLS_STANDARD").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, a PRIM_MEDIA_CONTROLS value meaning 'standard controls'"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_CONTROLS_MINI").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, a PRIM_MEDIA_CONTROLS value meaning 'mini controls'"
                            ;

                    GetLibraryConstantSignature("PRIM_MEDIA_PERM_NONE").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, a PRIM_MEDIA_PERMS_INTERACT or PRIM_MEDIA_PERMS_CONTROL bit, no permissions"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_PERM_OWNER").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, a PRIM_MEDIA_PERMS_INTERACT or PRIM_MEDIA_PERMS_CONTROL bit, owner permissions"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_PERM_GROUP").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, a PRIM_MEDIA_PERMS_INTERACT or PRIM_MEDIA_PERMS_CONTROL bit, group permissions"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_PERM_ANYONE").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, a PRIM_MEDIA_PERMS_INTERACT or PRIM_MEDIA_PERMS_CONTROL bit, anyone has permissions"
                            ;

                    GetLibraryConstantSignature("PRIM_MEDIA_MAX_URL_LENGTH").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, the maximum length of PRIM_MEDIA_CURRENT_URL or PRIM_MEDIA_HOME_URL"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_MAX_WHITELIST_SIZE").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, the maximum length, in bytes, of PRIM_MEDIA_WHITELIST"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_MAX_WHITELIST_COUNT").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, the maximum number of items allowed in PRIM_MEDIA_WHITELIST"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_MAX_WIDTH_PIXELS").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, the maximum width allowed in PRIM_MEDIA_WIDTH_PIXELS"
                            ;
                    GetLibraryConstantSignature("PRIM_MEDIA_MAX_HEIGHT_PIXELS").DocumentationString =
                        "Used with ll{Get,Set}PrimMediaParams, the maximum width allowed in PRIM_MEDIA_HEIGHT_PIXELS"
                            ;

                    GetLibraryConstantSignature("STATUS_OK").DocumentationString =
                        "Result of function call was success";
                    GetLibraryConstantSignature("STATUS_MALFORMED_PARAMS").DocumentationString =
                        "Function was called with malformed params";
                    GetLibraryConstantSignature("STATUS_TYPE_MISMATCH").DocumentationString =
                        "Argument(s) passed to function had a type mismatch";
                    GetLibraryConstantSignature("STATUS_BOUNDS_ERROR").DocumentationString =
                        "Argument(s) passed to function had a bounds error";
                    GetLibraryConstantSignature("STATUS_NOT_FOUND").DocumentationString =
                        "Object or other item was not found";
                    GetLibraryConstantSignature("STATUS_NOT_SUPPORTED").DocumentationString =
                        "Feature not supported";
                    GetLibraryConstantSignature("STATUS_INTERNAL_ERROR").DocumentationString =
                        "An internal error occurred";
                    GetLibraryConstantSignature("STATUS_WHITELIST_FAILED").DocumentationString =
                        "URL failed to pass whitelist";

                    GetLibraryConstantSignature("KFM_COMMAND").DocumentationString =
                        "Option for llSetKeyframedMotion(), followed by one of KFM_CMD_STOP, KFM_CMD_PLAY, KFM_CMD_PAUSE. Note that KFM_COMMAND must be the only option in the list, and cannot be specified in the same function call that sets the keyframes list."
                            ;
                    GetLibraryConstantSignature("KFM_MODE").DocumentationString =
                        "Option for llSetKeyframedMotion(), used to specify the playback mode, followed by one of KFM_FORWARD, KFM_LOOP, KFM_PING_PONG or KFM_REVERSE."
                            ;
                    GetLibraryConstantSignature("KFM_DATA").DocumentationString =
                        "Option for llSetKeyframedMotion(), followed by a bitwise combination of KFM_TRANSLATION and KFM_ROTATION. If you specify one or the other, you should only include translations or rotations in your keyframe list."
                            ;
                    GetLibraryConstantSignature("KFM_FORWARD").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_MODE to specify the forward playback mode."
                            ;
                    GetLibraryConstantSignature("KFM_LOOP").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_MODE to specify the loop playback mode."
                            ;
                    GetLibraryConstantSignature("KFM_PING_PONG").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_MODE to specify the ping pong playback mode."
                            ;
                    GetLibraryConstantSignature("KFM_REVERSE").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_MODE to specify the reverse playback mode."
                            ;
                    GetLibraryConstantSignature("KFM_ROTATION").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_DATA, possibly as a bitwise combination with KFM_TRANSLATION."
                            ;
                    GetLibraryConstantSignature("KFM_TRANSLATION").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_DATA, possibly as a bitwise combination with KFM_ROTATION."
                            ;
                    GetLibraryConstantSignature("KFM_CMD_PLAY").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_COMMAND to play the motion.";
                    GetLibraryConstantSignature("KFM_CMD_STOP").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_COMMAND to stop the motion.";
                    GetLibraryConstantSignature("KFM_CMD_PAUSE").DocumentationString =
                        "Option for llSetKeyframedMotion(), used after KFM_COMMAND to pause the motion.";

                    GetLibraryConstantSignature("ESTATE_ACCESS_ALLOWED_AGENT_ADD").DocumentationString =
                        "Used with llManageEstateAccess to add an agent to this estate's allowed residents list."
                            ;
                    GetLibraryConstantSignature("ESTATE_ACCESS_ALLOWED_AGENT_REMOVE").DocumentationString =
                        "Used with llManageEstateAccess to remove an agent from this estate's allowed residents list."
                            ;
                    GetLibraryConstantSignature("ESTATE_ACCESS_ALLOWED_GROUP_ADD").DocumentationString =
                        "Used with llManageEstateAccess to add a group to this estate's allowed groups list."
                            ;
                    GetLibraryConstantSignature("ESTATE_ACCESS_ALLOWED_GROUP_REMOVE").DocumentationString =
                        "Used with llManageEstateAccess to remove a group from this estate's allowed groups list."
                            ;
                    GetLibraryConstantSignature("ESTATE_ACCESS_BANNED_AGENT_ADD").DocumentationString =
                        "Used with llManageEstateAccess to add an agent to this estate's banned residents list."
                            ;
                    GetLibraryConstantSignature("ESTATE_ACCESS_BANNED_AGENT_REMOVE").DocumentationString =
                        "Used with llManageEstateAccess to remove an agent from this estate's banned residents list."
                            ;

                    GetLibraryConstantSignature("OBJECT_PHYSICS_COST").DocumentationString =
                        "Used with llGetObjectDetails to get the physics cost.";
                    GetLibraryConstantSignature("OBJECT_PRIM_EQUIVALENCE").DocumentationString =
                        "Used with llGetObjectDetails to get the prim equivalence.";
                    GetLibraryConstantSignature("OBJECT_SERVER_COST").DocumentationString =
                        "Used with llGetObjectDetails to get the server cost.";
                    GetLibraryConstantSignature("OBJECT_STREAMING_COST").DocumentationString =
                        "Used with llGetObjectDetails to get the streaming (download) cost.";

                    GetLibraryConstantSignature("PRIM_PHYSICS_SHAPE_TYPE").DocumentationString =
                        "For primitive physics shape type. Followed with either PRIM_PHYSICS_SHAPE_PRIM, PRIM_PHYSICS_SHAPE_NONE or PRIM_PHYSICS_SHAPE_CONVEX."
                            ;
                    GetLibraryConstantSignature("PRIM_PHYSICS_SHAPE_PRIM").DocumentationString =
                        "Use the normal prim shape for physics (this is the default for all non-mesh objects)"
                            ;
                    GetLibraryConstantSignature("PRIM_PHYSICS_SHAPE_NONE").DocumentationString =
                        "Use the convex hull of the prim shape for physics (this is the default for mesh objects)"
                            ;
                    GetLibraryConstantSignature("PRIM_PHYSICS_SHAPE_CONVEX").DocumentationString =
                        "Ignore this prim in the physics shape. This cannot be applied to the root prim.";

                    GetLibraryConstantSignature("DENSITY").DocumentationString =
                        "For use with llSetPhysicsMaterial() as a bitwise value in its material_bits parameter, to set the density."
                            ;
                    GetLibraryConstantSignature("FRICTION").DocumentationString =
                        "For use with llSetPhysicsMaterial() as a bitwise value in its material_bits parameter, to set the friction."
                            ;
                    GetLibraryConstantSignature("RESTITUTION").DocumentationString =
                        "For use with llSetPhysicsMaterial() as a bitwise value in its material_bits parameter, to set the restitution."
                            ;
                    GetLibraryConstantSignature("GRAVITY_MULTIPLIER").DocumentationString =
                        "For use with llSetPhysicsMaterial() as a bitwise value in its material_bits parameter, to set the gravity multiplier."
                            ;

                    GetLibraryConstantSignature("PROFILE_SCRIPT_NONE").DocumentationString =
                        "Disables memory profiling when passed to llScriptProfiler()";
                    GetLibraryConstantSignature("PROFILE_SCRIPT_MEMORY").DocumentationString =
                        "Enables memory profiling when passed to llScriptProfiler()";

                    GetLibraryConstantSignature("RCERR_UNKNOWN").DocumentationString =
                        "Returned by llCastRay() when the raycast failed for an unspecified reason.";
                    GetLibraryConstantSignature("RCERR_SIM_PERF_LOW").DocumentationString =
                        "Returned by llCastRay() when the raycast failed because simulator performance is low."
                            ;
                    GetLibraryConstantSignature("RCERR_CAST_TIME_EXCEEDED").DocumentationString =
                        "Returned by llCastRay() when the raycast failed because the parcel or agent has exceeded the maximum time allowed for raycasting."
                            ;

                    GetLibraryConstantSignature("RC_DETECT_PHANTOM").DocumentationString =
                        "Option for llCastRay() followed with TRUE to detect phantom AND volume detect objects, FASLE otherwise."
                            ;
                    GetLibraryConstantSignature("RC_DATA_FLAGS").DocumentationString =
                        "Option for llCastRay() followed with a bitwise combination of RC_GET_NORMAL, RC_GET_ROOT_KEY and RC_GET_LINK_NUM."
                            ;
                    GetLibraryConstantSignature("RC_MAX_HITS").DocumentationString =
                        "Option for llCastRay() followed with an integer specifying the maximum number of hits to return (must be &lt;= 256)."
                            ;
                    GetLibraryConstantSignature("RC_GET_NORMAL").DocumentationString =
                        "Flag used in the RC_DATA_FLAGS mask to get hit normals in llCastRay() results.";
                    GetLibraryConstantSignature("RC_GET_ROOT_KEY").DocumentationString =
                        "Flag used in the RC_DATA_FLAGS mask to get root keys in llCastRay() results.";
                    GetLibraryConstantSignature("RC_GET_LINK_NUM").DocumentationString =
                        "Flag used in the RC_DATA_FLAGS mask to get link numbers in llCastRay() results.";
                    GetLibraryConstantSignature("RC_REJECT_TYPES").DocumentationString =
                        "Optional parameter set in llCastRay() to reject hit against certain object types.";
                    GetLibraryConstantSignature("RC_REJECT_AGENTS").DocumentationString =
                        "Bit mask for RC_REJECT_TYPES, rejects hits against avatars.";
                    GetLibraryConstantSignature("RC_REJECT_PHYSICAL").DocumentationString =
                        "Bit mask for RC_REJECT_TYPES, rejects hits against moving objects.";
                    GetLibraryConstantSignature("RC_REJECT_NONPHYSICAL").DocumentationString =
                        "Bit mask for RC_REJECT_TYPES, rejects hits against non-moving objects.";
                    GetLibraryConstantSignature("RC_REJECT_LAND").DocumentationString =
                        "Bit mask for RC_REJECT_TYPES, rejects hits against the terrian.";

                    GetLibraryConstantSignature("SIM_STAT_PCT_CHARS_STEPPED").DocumentationString =
                        "Option for llGetSimStats() to return the % of pathfinding characters skipped each frame, averaged over the last minute."
                            ;

                    GetLibraryConstantSignature("CHARACTER_DESIRED_SPEED").DocumentationString =
                        "Speed of pursuit in meters per second.";
                    GetLibraryConstantSignature("CHARACTER_RADIUS").DocumentationString =
                        "Set collision capsule radius.";
                    GetLibraryConstantSignature("CHARACTER_LENGTH").DocumentationString =
                        "Set collision capsule length.";
                    GetLibraryConstantSignature("CHARACTER_ORIENTATION").DocumentationString =
                        "Set the character orientation.";
                    GetLibraryConstantSignature("VERTICAL").DocumentationString =
                        "Constant to indicate that the orientation of the capsule for a Pathfinding character is vertical."
                            ;
                    GetLibraryConstantSignature("HORIZONTAL").DocumentationString =
                        "Constant to indicate that the orientation of the capsule for a Pathfinding character is horizontal."
                            ;
                    GetLibraryConstantSignature("TRAVERSAL_TYPE").DocumentationString =
                        "Controls the speed at which characters moves on terrain that is less than 100% walkable will move faster (e.g., a cat crossing a street) or slower (e.g., a car driving in a swamp)."
                            ;
                    GetLibraryConstantSignature("CHARACTER_TYPE").DocumentationString =
                        "Specifies which walkability coefficient will be used by this character.";
                    GetLibraryConstantSignature("CHARACTER_TYPE_A").DocumentationString =
                        "Used for character types that you prefer move in a way consistent with humanoids.";
                    GetLibraryConstantSignature("CHARACTER_TYPE_B").DocumentationString =
                        "Used for character types that you prefer move in a way consistent with wild animals or off road vehicles."
                            ;
                    GetLibraryConstantSignature("CHARACTER_TYPE_C").DocumentationString =
                        "Used for mechanical character types or road going vehicles.";
                    GetLibraryConstantSignature("CHARACTER_TYPE_D").DocumentationString =
                        "Used for character types that are not consistent with the A, B, or C type.";
                    GetLibraryConstantSignature("CHARACTER_TYPE_NONE").DocumentationString =
                        "Used to set no specific character type.";

                    GetLibraryConstantSignature("CHARACTER_AVOIDANCE_MODE").DocumentationString =
                        "Allows you to specify that a character should not try to avoid other characters, should not try to avoid dynamic obstacles (relatively fast moving objects and avatars), or both."
                            ;
                    GetLibraryConstantSignature("CHARACTER_MAX_ACCEL").DocumentationString =
                        "The character's maximum acceleration rate.";
                    GetLibraryConstantSignature("CHARACTER_MAX_DECEL").DocumentationString =
                        "The character's maximum deceleration rate.";

                    GetLibraryConstantSignature("CHARACTER_DESIRED_TURN_SPEED").DocumentationString =
                        "The character's maximum speed while turning--note that this is only loosely enforced (i.e., a character may turn at higher speeds under certain conditions)"
                            ;
                    GetLibraryConstantSignature("CHARACTER_MAX_TURN_RADIUS").DocumentationString =
                        "The character's turn radius when traveling at CHARACTER_DESIRED_TURN_SPEED.";
                    GetLibraryConstantSignature("CHARACTER_MAX_SPEED").DocumentationString =
                        "The character's maximum speed. Affects speed when avoiding dynamic obstacles and when traversing low-walkability objects in TRAVERSAL_TYPE_FAST mode."
                            ;
                    GetLibraryConstantSignature("CHARACTER_STAY_WITHIN_PARCEL").DocumentationString =
                        "Characters which have CHARACTER_STAY_WITHIN_PARCEL set to TRUE treat the parcel boundaries as one-way obstacles."
                            ;

                    GetLibraryConstantSignature("CHARACTER_ACCOUNT_FOR_SKIPPED_FRAMES").DocumentationString =
                        "Defines if a character will attempt to catch up lost time if pathfinding performance is low."
                            ;
                    GetLibraryConstantSignature("OBJECT_PATHFINDING_TYPE").DocumentationString =
                        "Used with llGetObjectDetails to get an object's pathfinding settings.";
                    GetLibraryConstantSignature("OPT_UNKNOWN").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for attachments, Linden trees and grass."
                            ;
                    GetLibraryConstantSignature("OPT_OTHER").DocumentationString =
                        "Returned for attachments, Linden trees and grass, was OPT_UNKOWN";
                    GetLibraryConstantSignature("OPT_LEGACY_LINKSET").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for movable obstacles, movable phantoms, physical, and volumedetect objects."
                            ;
                    GetLibraryConstantSignature("OPT_AVATAR").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for avatars.";
                    GetLibraryConstantSignature("OPT_PATHFINDING_CHARACTER").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for pathfinding characters.".UnescapeXml
                            ();
                    GetLibraryConstantSignature("OOPT_CHARACTER").DocumentationString =
                        "Returned for pathfinding characters, was OPT_PATHFINDING_CHARACTER";
                    GetLibraryConstantSignature("OPT_WALKABLE").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for walkable objects.";
                    GetLibraryConstantSignature("OPT_STATIC_OBSTACLE").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for static obstacles.";
                    GetLibraryConstantSignature("OPT_MATERIAL_VOLUME").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for material volumes.";
                    GetLibraryConstantSignature("OPT_EXCLUSION_VOLUME").DocumentationString =
                        "Returned object pathfinding type by llGetObjectDetails for exclusion volumes.";

                    GetLibraryConstantSignature("PATROL_PAUSE_AT_WAYPOINTS").DocumentationString =
                        "Used with llPatrolPoints(). Defines if characters slow down and momentarily pause at each waypoint."
                            ;
                    GetLibraryConstantSignature("WANDER_PAUSE_AT_WAYPOINTS").DocumentationString =
                        "Used with llWanderWithin(). Defines if characters should pause after reaching each wander waypoint."
                            ;

                    GetLibraryConstantSignature("PURSUIT_OFFSET").DocumentationString =
                        "Used with llPursue(). Go to a position offset from the target.";
                    GetLibraryConstantSignature("REQUIRE_LINE_OF_SIGHT").DocumentationString =
                        "Used with llPursue(). Define whether the character needs a physical line-of-sight to give chase. When enabled, the character will not pick a new target position while there is a something solid between the character and the target object/agent."
                            ;
                    GetLibraryConstantSignature("PURSUIT_FUZZ_FACTOR").DocumentationString =
                        "Used with llPursue(). Selects a random destination near the PURSUIT_OFFSET. The valid fuzz factor range is from 0 to 1, where 1 is most random. This option requires a nonzero PURSUIT_OFFSET."
                            ;
                    GetLibraryConstantSignature("PURSUIT_INTERCEPT").DocumentationString =
                        "Used with llPursue(). Define whether the character attempts to predict the target's future location."
                            ;
                    GetLibraryConstantSignature("PURSUIT_GOAL_TOLERANCE").DocumentationString =
                        "Used with llPursue(). Defines approximately how close the character must be to the current goal to consider itself to be at the desired position. The valid range is from 0.25 to 10m."
                            ;
                    GetLibraryConstantSignature("CHARACTER_CMD_STOP").DocumentationString =
                        "Used with  llExecCharacterCmd(). Makes the character jump.";
                    GetLibraryConstantSignature("CHARACTER_CMD_SMOOTH_STOP").DocumentationString =
                        "Used with  llExecCharacterCmd(). Stops any current pathfinding operation in a smooth like fashion."
                            ;
                    GetLibraryConstantSignature("CHARACTER_CMD_JUMP").DocumentationString =
                        "Used with  llExecCharacterCmd(). Stops any current pathfinding operation.";

                    GetLibraryConstantSignature("PU_EVADE_HIDDEN").DocumentationString =
                        "Triggered when an llEvade character thinks it has hidden from its pursuer.";
                    GetLibraryConstantSignature("PU_EVADE_SPOTTED").DocumentationString =
                        "Triggered when an llEvade character switches from hiding to running";
                    GetLibraryConstantSignature("PU_FAILURE_INVALID_GOAL").DocumentationString =
                        "Goal is not on the navigation-mesh and cannot be reached.";
                    GetLibraryConstantSignature("PU_FAILURE_INVALID_START").DocumentationString =
                        "Character cannot navigate from the current location - e.g., the character is off the navmesh or too high above it."
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_NO_VALID_DESTINATION").DocumentationString =
                        "There's no good place for the character to go - e.g., it is patrolling and all the patrol points are now unreachable."
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_OTHER").DocumentationString =
                        "Unknown failure";
                    GetLibraryConstantSignature("PU_FAILURE_TARGET_GONE").DocumentationString =
                        "Target (for llPursue or llEvade) can no longer be tracked - e.g., it left the region or is an avatar that is now more than about 30m outside the region."
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_UNREACHABLE").DocumentationString =
                        "Goal is no longer reachable for some reason - e.g., an obstacle blocks the path.";
                    GetLibraryConstantSignature("PU_GOAL_REACHED").DocumentationString =
                        "Character has reached the goal and will stop or choose a new goal (if wandering).";
                    GetLibraryConstantSignature("PU_SLOWDOWN_DISTANCE_REACHED").DocumentationString =
                        "Character is near current goal.";
                    GetLibraryConstantSignature("PU_FAILURE_NO_NAVMESH").DocumentationString =
                        "Triggered if no navmesh is available for the region.";
                    GetLibraryConstantSignature("PU_FAILURE_DYNAMIC_PATHFINDING_DISABLED").DocumentationString =
                        "Triggered when a character enters a region with dynamic pathfinding disabled.";
                    GetLibraryConstantSignature("PU_FAILURE_PARCEL_UNREACHABLE").DocumentationString =
                        "Triggered when a character failed to enter a parcel because it is not allowed to enter, e.g. because the parcel is already full or because object entry was disabled after the navmesh was baked."
                            ;

                    GetLibraryConstantSignature("JSON_INVALID").DocumentationString =
                        "A return value that indicates an invalid type was specified to an llJson* function".UnescapeXml
                            ();
                    GetLibraryConstantSignature("JSON_OBJECT").DocumentationString =
                        "Represents a json datatype represented in LSL as a strided list of name/value pairs"
                            ;
                    GetLibraryConstantSignature("JSON_ARRAY").DocumentationString =
                        "Represents a json datatype mappable to the LSL datatype &quot;list&quot;";
                    GetLibraryConstantSignature("JSON_NUMBER").DocumentationString =
                        "Represents a json datatype mappable to the LSL datatypes &quot;integer&quot; and &quot;float&quot;"
                            ;
                    GetLibraryConstantSignature("JSON_STRING").DocumentationString =
                        "Represents a json datatype mappable to the LSL datatype &quot;string&quot;";
                    GetLibraryConstantSignature("JSON_TRUE").DocumentationString =
                        "Represents the constant &quot;true&quot; of a json value.";
                    GetLibraryConstantSignature("JSON_FALSE").DocumentationString =
                        "Represents the constant &quot;false&quot; of a json value.";
                    GetLibraryConstantSignature("JSON_NULL").DocumentationString =
                        "Represents the constant &quot;null&quot; of a json value.";
                    GetLibraryConstantSignature("JSON_APPEND").DocumentationString =
                        "Used with llJsonSetValue as a specifier to indicate appending the value to the end of the array at that level."
                            ;
                    GetLibraryConstantSignature("JSON_DELETE").DocumentationString =
                        "Used to delete a value within a JSON text string.";

                    GetLibraryConstantSignature("ERR_GENERIC").DocumentationString =
                        "Returned by llReturnObjectsByID and llReturnObjectsByOwner in case of a general error."
                            ;
                    GetLibraryConstantSignature("ERR_PARCEL_PERMISSIONS").DocumentationString =
                        "Returned by llReturnObjectsByID and llReturnObjectsByOwner in case of a parcel owner permission error."
                            ;
                    GetLibraryConstantSignature("ERR_MALFORMED_PARAMS").DocumentationString =
                        "Returned by llReturnObjectsByID and llReturnObjectsByOwner in case of malformed parameters."
                            ;
                    GetLibraryConstantSignature("ERR_RUNTIME_PERMISSIONS").DocumentationString =
                        "Returned by llReturnObjectsByID and llReturnObjectsByOwner in case of a runtime permission error."
                            ;
                    GetLibraryConstantSignature("ERR_THROTTLED").DocumentationString =
                        "Returned by llReturnObjectsByID and llReturnObjectsByOwner in case of being throttled."
                            ;



                    GetLibraryConstantSignature("PROFILE_NONE").DocumentationString = "Disables profiling";
                    GetLibraryConstantSignature("PROFILE_SCRIPT_MEMORY").DocumentationString =
                        "Enables memory profiling";

                    GetLibraryConstantSignature("PU_SLOWDOWN_DISTANCE_REACHED").DocumentationString =
                        "Character is near current goal";
                    GetLibraryConstantSignature("PU_GOAL_REACHED").DocumentationString =
                        "Character has reached the goal and will stop or choose a new goal (if wandering)";
                    GetLibraryConstantSignature("PU_FAILURE_INVALID_START").DocumentationString =
                        "Character cannot navigate from the current location - e.g., the character is off the navmesh or too high above it"
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_INVALID_GOAL").DocumentationString =
                        "Goal is not on the navmesh and cannot be reached";
                    GetLibraryConstantSignature("PU_FAILURE_UNREACHABLE").DocumentationString =
                        "Goal is no longer reachable for some reason - e.g., an obstacle blocks the path";
                    GetLibraryConstantSignature("PU_FAILURE_TARGET_GONE").DocumentationString =
                        "Target (for llPursue or llEvade) can no longer be tracked - e.g., it left the region or is an avatar that is now more than about 30m outside the region"
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_NO_VALID_DESTINATION").DocumentationString =
                        "There's no good place for the character to go - e.g., it is patrolling and all the patrol points are now unreachable"
                            ;
                    GetLibraryConstantSignature("PU_EVADE_HIDDEN").DocumentationString =
                        "Triggered when an llEvade character thinks it has hidden from its pursuer";
                    GetLibraryConstantSignature("PU_EVADE_SPOTTED").DocumentationString =
                        "Triggered when an llEvade character switches from hiding to running";
                    GetLibraryConstantSignature("PU_FAILURE_NO_NAVMESH").DocumentationString =
                        "This is a fatal error reported to a character when there is no navmesh for the region. This usually indicates a server failure and users should file a bug report and include the time and region in which they received this message"
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_DYNAMIC_PATHFINDING_DISABLED").DocumentationString =
                        "Triggered when a character enters a region with dynamic pathfinding disabled. Dynamic pathfinding can be toggled by estate managers via the 'dynamic_pathfinding' option in the Region Debug Console"
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_PARCEL_UNREACHABLE").DocumentationString =
                        "Triggered when a character failed to enter a parcel because it is not allowed to enter, e.g. because the parcel is already full or because object entry was disabled after the navmesh was baked"
                            ;
                    GetLibraryConstantSignature("PU_FAILURE_OTHER").DocumentationString = "Other failure";

                    GetLibraryFunctionSignature("llSin").DocumentationString =
                        "Returns the sine of theta (theta in radians)";
                    GetLibraryFunctionSignature("llCos").DocumentationString =
                        "Returns the cosine of theta (theta in radians)";
                    GetLibraryFunctionSignature("llAcos").DocumentationString =
                        "Returns the arccosine in radians of val";
                    GetLibraryFunctionSignature("llTan").DocumentationString =
                        "Returns the tangent of theta (theta in radians)";
                    GetLibraryFunctionSignature("llAtan2").DocumentationString =
                        "Returns the arctangent2 of y, x";
                    GetLibraryFunctionSignature("llAsin").DocumentationString =
                        "Returns the arcsine in radians of val";
                    GetLibraryFunctionSignature("llSqrt").DocumentationString =
                        "Returns the square root of val, or returns 0 and triggers a Math Error for imaginary results"
                            ;
                    GetLibraryFunctionSignature("llPow").DocumentationString =
                        "Returns the base raised to the power exponent, or returns 0 and triggers Math Error for imaginary results"
                            ;
                    GetLibraryFunctionSignature("llModPow").DocumentationString =
                        "Returns a raised to the b power, mod c. ( (a**b)%c );b is capped at 0xFFFF (16 bits)."
                            ;
                    GetLibraryFunctionSignature("llAbs").DocumentationString =
                        "Returns the positive version of val";
                    GetLibraryFunctionSignature("llFabs").DocumentationString =
                        "Returns the positive version of val";
                    GetLibraryFunctionSignature("llFrand").DocumentationString =
                        "Returns a pseudo random number in the range [0,mag) or (mag,0]";
                    GetLibraryFunctionSignature("llFloor").DocumentationString =
                        "Returns largest integer value &lt;= val";
                    GetLibraryFunctionSignature("llCeil").DocumentationString =
                        "Returns smallest integer value &gt;= val";
                    GetLibraryFunctionSignature("llRound").DocumentationString =
                        "Returns val rounded to the nearest integer";
                    GetLibraryFunctionSignature("llVecMag").DocumentationString =
                        "Returns the magnitude of v";
                    GetLibraryFunctionSignature("llVecNorm").DocumentationString =
                        "Returns the v normalized";
                    GetLibraryFunctionSignature("llVecDist").DocumentationString =
                        "Returns the 3D distance between v1 and v2";
                    GetLibraryFunctionSignature("llRot2Euler").DocumentationString =
                        "Returns the Euler representation (roll, pitch, yaw) of q";
                    GetLibraryFunctionSignature("llEuler2Rot").DocumentationString =
                        "Returns the rotation representation of Euler Angles v";
                    GetLibraryFunctionSignature("llAxes2Rot").DocumentationString =
                        "Returns the rotation defined by the coordinate axes";
                    GetLibraryFunctionSignature("llRot2Fwd").DocumentationString =
                        "Returns the forward vector defined by q";
                    GetLibraryFunctionSignature("llRot2Left").DocumentationString =
                        "Returns the left vector defined by q";
                    GetLibraryFunctionSignature("llRot2Up").DocumentationString =
                        "Returns the up vector defined by q";
                    GetLibraryFunctionSignature("llRotBetween").DocumentationString =
                        "Returns the rotation to rotate v1 to v2";
                    GetLibraryFunctionSignature("llRot2Axis").DocumentationString =
                        "Returns the rotation axis represented by rot";
                    GetLibraryFunctionSignature("llRot2Angle").DocumentationString =
                        "Returns the rotation angle represented by rot";
                    GetLibraryFunctionSignature("llAxisAngle2Rot").DocumentationString =
                        "Returns the rotation that is a generated angle about axis";
                    GetLibraryFunctionSignature("llAngleBetween").DocumentationString =
                        "Returns angle between rotation a and b";
                    GetLibraryFunctionSignature("llWhisper").DocumentationString =
                        "Whispers the text of message on channel";
                    GetLibraryFunctionSignature("llSay").DocumentationString =
                        "Says the text of message on channel";
                    GetLibraryFunctionSignature("llShout").DocumentationString =
                        "Shouts the text of message on channel";
                    GetLibraryFunctionSignature("llListen").DocumentationString =
                        "Sets a callback for message on channel from name and id (name, id, and/or message can be empty) and returns an identifier that can be used to deactivate or remove the listen"
                            ;
                    GetLibraryFunctionSignature("llListenControl").DocumentationString =
                        "Makes a listen event callback active or inactive";
                    GetLibraryFunctionSignature("llListenRemove").DocumentationString =
                        "Removes listen event callback number";
                    GetLibraryFunctionSignature("llSensor").DocumentationString =
                        "Performs a single scan for name and id with type (AGENT, ACTIVE, PASSIVE, and/or SCRIPTED) within range meters and arc radians of forward vector (name, id, and/or keytype can be empty or 0)"
                            ;
                    GetLibraryFunctionSignature("llSensorRepeat").DocumentationString =
                        "Sets a callback for name and id with type (AGENT, ACTIVE, PASSIVE, and/or SCRIPTED) within range meters and arc radians of forward vector (name, id, and/or keytype can be empty or 0) and repeats every rate seconds"
                            ;
                    GetLibraryFunctionSignature("llSensorRemove").DocumentationString =
                        "Removes the sensor setup by llSensorRepeat";
                    GetLibraryFunctionSignature("llDetectedName").DocumentationString =
                        "Returns the name of detected object number (returns empty string if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedKey").DocumentationString =
                        "Returns the key of detected object number (returns empty key if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedOwner").DocumentationString =
                        "Returns the key of detected object&apos;s owner (returns empty key if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedType").DocumentationString =
                        "Returns the type (AGENT, ACTIVE, PASSIVE, SCRIPTED) of detected object (returns 0 if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedPos").DocumentationString =
                        "Returns the position of detected object number (returns &lt;0,0,0&gt; if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedVel").DocumentationString =
                        "Returns the velocity of detected object number (returns &lt;0,0,0&gt; if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedGrab").DocumentationString =
                        "Returns the grab offset of the user touching object (returns &lt;0,0,0&gt; if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedRot").DocumentationString =
                        "Returns the rotation of detected object number (returns &lt;0,0,0,1&gt; if number is not a valid sensed object)"
                            ;
                    GetLibraryFunctionSignature("llDetectedGroup").DocumentationString =
                        "Returns TRUE if detected object is part of same group as owner";
                    GetLibraryFunctionSignature("llDetectedLinkNumber").DocumentationString =
                        "Returns the integer link number of the triggered event. If not supported, returns zero"
                            ;
                    GetLibraryFunctionSignature("llDie").DocumentationString = "Deletes the object";
                    GetLibraryFunctionSignature("llGround").DocumentationString =
                        "Returns the ground height below the object position + offset";
                    GetLibraryFunctionSignature("llCloud").DocumentationString =
                        "Returns the cloud density at the object position + offset";
                    GetLibraryFunctionSignature("llWind").DocumentationString =
                        "Returns the wind velocity at the object position + offset";
                    GetLibraryFunctionSignature("llWater").DocumentationString =
                        "Returns the water height below the object position + offset";
                    GetLibraryFunctionSignature("llSetStatus").DocumentationString =
                        "Sets status (STATUS_PHYSICS, STATUS_PHANTOM, STATUS_BLOCK_GRAB, STATUS_ROTATE_X, STATUS_ROTATE_Y, and/or STATUS_ROTATE_Z) to value"
                            ;
                    GetLibraryFunctionSignature("llGetStatus").DocumentationString =
                        "Returns value of status (STATUS_PHYSICS, STATUS_PHANTOM, STATUS_BLOCK_GRAB, STATUS_ROTATE_X, STATUS_ROTATE_Y, and/or STATUS_ROTATE_Z)"
                            ;
                    GetLibraryFunctionSignature("llSetScale").DocumentationString =
                        "Sets the scale of the prim";
                    GetLibraryFunctionSignature("llGetScale").DocumentationString =
                        "Returns the scale of the prim";
                    GetLibraryFunctionSignature("llSetColor").DocumentationString =
                        "Sets the color on face of the prim";
                    GetLibraryFunctionSignature("llGetAlpha").DocumentationString =
                        "Returns the alpha of face";
                    GetLibraryFunctionSignature("llSetAlpha").DocumentationString =
                        "Sets the alpha on face";
                    GetLibraryFunctionSignature("llGetColor").DocumentationString =
                        "Returns the color on face";
                    GetLibraryFunctionSignature("llSetTexture").DocumentationString =
                        "Sets the texture of face or ALL_SIDES";
                    GetLibraryFunctionSignature("llScaleTexture").DocumentationString =
                        "Sets the texture u &amp; v scales for the chosen face or ALL_SIDES";
                    GetLibraryFunctionSignature("llOffsetTexture").DocumentationString =
                        "Sets the texture u  &amp; v offsets for the chosen face or ALL_SIDES";
                    GetLibraryFunctionSignature("llRotateTexture").DocumentationString =
                        "Sets the texture rotation for the chosen face";
                    GetLibraryFunctionSignature("llGetTexture").DocumentationString =
                        "Returns a string that is the texture on face (the inventory name if it is a texture in the prim&apos;s inventory, otherwise the key)"
                            ;
                    GetLibraryFunctionSignature("llSetPos").DocumentationString =
                        "Moves the object or prim towards pos without using physics (if the script isn&apos;t physical)"
                            ;
                    GetLibraryFunctionSignature("llGetPos").DocumentationString =
                        "Returns the position of the task in region coordinates";
                    GetLibraryFunctionSignature("llGetLocalPos").DocumentationString =
                        "Returns the position relative to the root";
                    GetLibraryFunctionSignature("llSetRot").DocumentationString = "Sets the rotation";
                    GetLibraryFunctionSignature("llGetRot").DocumentationString =
                        "Returns the rotation relative to the region&apos;s axes";
                    GetLibraryFunctionSignature("llGetLocalRot").DocumentationString =
                        "Returns the rotation local to the root";
                    GetLibraryFunctionSignature("llSetForce").DocumentationString =
                        "Applies force to the object (if the script is physical), in local coords if local == TRUE"
                            ;
                    GetLibraryFunctionSignature("llGetForce").DocumentationString =
                        "Returns the force (if the script is physical)";
                    GetLibraryFunctionSignature("llTarget").DocumentationString =
                        "Sets positions within range of position as a target and return an ID for the target"
                            ;
                    GetLibraryFunctionSignature("llTargetRemove").DocumentationString =
                        "Removes positional target number registered with llTarget";
                    GetLibraryFunctionSignature("llRotTarget").DocumentationString =
                        "Set rotations with error of rot as a rotational target and return an ID for the rotational target"
                            ;
                    GetLibraryFunctionSignature("llRotTargetRemove").DocumentationString =
                        "Removes rotational target number registered with llRotTarget";
                    GetLibraryFunctionSignature("llMoveToTarget").DocumentationString =
                        "Critically damps to target in tau seconds (if the script is physical)";
                    GetLibraryFunctionSignature("llStopMoveToTarget").DocumentationString =
                        "Stops critically damped motion";
                    GetLibraryFunctionSignature("llApplyImpulse").DocumentationString =
                        "Applies impulse to object (if the script is physical), in local coords if local == TRUE"
                            ;
                    GetLibraryFunctionSignature("llApplyRotationalImpulse").DocumentationString =
                        "Applies rotational impulse to object (if the script is physical), in local coords if local == TRUE"
                            ;
                    GetLibraryFunctionSignature("llSetTorque").DocumentationString =
                        "Sets the torque of object (if the script is physical), in local coords if local == TRUE"
                            ;
                    GetLibraryFunctionSignature("llGetTorque").DocumentationString =
                        "Returns the torque (if the script is physical)";
                    GetLibraryFunctionSignature("llSetForceAndTorque").DocumentationString =
                        "Sets the force and torque of object (if the script is physical), in local coords if local == TRUE"
                            ;
                    GetLibraryFunctionSignature("llGetVel").DocumentationString =
                        "Returns the velocity of the object";
                    GetLibraryFunctionSignature("llGetAccel").DocumentationString =
                        "Returns the acceleration of the object relative to the region&apos;s axes";
                    GetLibraryFunctionSignature("llGetOmega").DocumentationString =
                        "Returns the rotation velocity in radians per second";
                    GetLibraryFunctionSignature("llGetTimeOfDay").DocumentationString =
                        "Returns the time in seconds since grid server midnight or since region up-time, whichever is smaller"
                            ;
                    GetLibraryFunctionSignature("llGetWallclock").DocumentationString =
                        "Returns the time in seconds since midnight California Pacific time (PST/PDT)";
                    GetLibraryFunctionSignature("llGetUnixTime").DocumentationString =
                        "Returns the number of seconds elapsed since 00;00 hours, Jan 1, 1970 UTC from the system clock"
                            ;
                    GetLibraryFunctionSignature("llGetTime").DocumentationString =
                        "Returns the time in seconds since the last region reset, script reset, or call to either llResetTime or llGetAndResetTime"
                            ;
                    GetLibraryFunctionSignature("llResetTime").DocumentationString =
                        "Sets the script timer to zero";
                    GetLibraryFunctionSignature("llGetAndResetTime").DocumentationString =
                        "Returns the script time in seconds and then resets the script timer to zero";
                    GetLibraryFunctionSignature("llSound").DocumentationString =
                        "Plays sound at volume and whether it should loop or not";
                    GetLibraryFunctionSignature("llPlaySound").DocumentationString =
                        "Plays attached sound once at volume (0.0 - 1.0)";
                    GetLibraryFunctionSignature("llLoopSound").DocumentationString =
                        "Plays attached sound looping indefinitely at volume (0.0 - 1.0)";
                    GetLibraryFunctionSignature("llLoopSoundMaster").DocumentationString =
                        "Plays attached sound looping at volume (0.0 - 1.0), declares it a sync master";
                    GetLibraryFunctionSignature("llLoopSoundSlave").DocumentationString =
                        "Plays attached sound looping at volume (0.0 - 1.0), synced to most audible sync master"
                            ;
                    GetLibraryFunctionSignature("llPlaySoundSlave").DocumentationString =
                        "Plays attached sound once at volume (0.0 - 1.0), synced to next loop of most audible sync master"
                            ;
                    GetLibraryFunctionSignature("llTriggerSound").DocumentationString =
                        "Plays sound at volume (0.0 - 1.0), centered at but not attached to object";
                    GetLibraryFunctionSignature("llStopSound").DocumentationString =
                        "Stops currently attached sound";
                    GetLibraryFunctionSignature("llPreloadSound").DocumentationString =
                        "Preloads a sound on viewers within range";
                    GetLibraryFunctionSignature("llGetSubString").DocumentationString =
                        "Returns the indicated substring";
                    GetLibraryFunctionSignature("llDeleteSubString").DocumentationString =
                        "Removes the indicated substring and returns the result";
                    GetLibraryFunctionSignature("llInsertString").DocumentationString =
                        "Returns a destination string dst with the string src inserted starting at position pos"
                            ;
                    GetLibraryFunctionSignature("llToUpper").DocumentationString =
                        "Returns a string that is src with all upper-case characters";
                    GetLibraryFunctionSignature("llToLower").DocumentationString =
                        "Returns a string that is src with all lower-case characters";
                    GetLibraryFunctionSignature("llGiveMoney").DocumentationString =
                        "Transfers amount of L$ from script owner to destination";
                    GetLibraryFunctionSignature("llMakeExplosion").DocumentationString =
                        "Makes a round explosion of particles";
                    GetLibraryFunctionSignature("llMakeFountain").DocumentationString =
                        "Makes a fountain of particles";
                    GetLibraryFunctionSignature("llMakeSmoke").DocumentationString =
                        "Makes smoke like particles";
                    GetLibraryFunctionSignature("llMakeFire").DocumentationString =
                        "Makes fire like particles";
                    GetLibraryFunctionSignature("llRezObject").DocumentationString =
                        "Instantiates owner&apos;s inventory object at pos with velocity vel and rotation rot with start parameter param"
                            ;
                    GetLibraryFunctionSignature("llLookAt").DocumentationString =
                        "Causes object to point its up axis (positive z) towards target, while keeping its forward axis (positive x) below the horizon"
                            ;
                    GetLibraryFunctionSignature("llStopLookAt").DocumentationString =
                        "Stops causing object to point at a target";
                    GetLibraryFunctionSignature("llSetTimerEvent").DocumentationString =
                        "Causes the timer event to be triggered a maximum of once every sec seconds";
                    GetLibraryFunctionSignature("llSleep").DocumentationString =
                        "Puts the script to sleep for sec seconds";
                    GetLibraryFunctionSignature("llGetMass").DocumentationString =
                        "Returns the mass of object that the script is attached to";
                    GetLibraryFunctionSignature("llCollisionFilter").DocumentationString =
                        "Sets the collision filter, exclusively or inclusively. If accept == TRUE, only accept collisions with objects name and id (either is optional), otherwise with objects not name or id"
                            ;
                    GetLibraryFunctionSignature("llTakeControls").DocumentationString =
                        "Allows for intercepting keyboard and mouse clicks from the agent the script has permissions for"
                            ;
                    GetLibraryFunctionSignature("llReleaseControls").DocumentationString =
                        "Stops taking inputs that were taken with llTakeControls";
                    GetLibraryFunctionSignature("llAttachToAvatar").DocumentationString =
                        "Attaches the object to the avatar who has granted permission to the script";
                    GetLibraryFunctionSignature("llDetachFromAvatar").DocumentationString =
                        "Detaches object from avatar";
                    GetLibraryFunctionSignature("llTakeCamera").DocumentationString =
                        "Moves avatar&apos;s viewpoint to task";
                    GetLibraryFunctionSignature("llReleaseCamera").DocumentationString =
                        "Returns camera to agent avatar";
                    GetLibraryFunctionSignature("llGetOwner").DocumentationString =
                        "Returns the object owner&apos;s UUID";
                    GetLibraryFunctionSignature("llInstantMessage").DocumentationString =
                        "Sends the specified string as an Instant Message to the user";
                    GetLibraryFunctionSignature("llEmail").DocumentationString =
                        "Sends an email to address with the subject and message";
                    GetLibraryFunctionSignature("llGetNextEmail").DocumentationString =
                        "Gets the next waiting email that comes from address, with specified subject";
                    GetLibraryFunctionSignature("llGetKey").DocumentationString =
                        "Returns the key of the prim the script is attached to";
                    GetLibraryFunctionSignature("llSetBuoyancy").DocumentationString =
                        "Sets the buoyancy of the task or object (0 is disabled, &lt; 1.0 sinks, 1.0 floats, &gt; 1.0 rises)"
                            ;
                    GetLibraryFunctionSignature("llSetHoverHeight").DocumentationString =
                        "Critically damps to a height above the ground (or water) in tau seconds";
                    GetLibraryFunctionSignature("llStopHover").DocumentationString =
                        "Stops hovering to a height";
                    GetLibraryFunctionSignature("llMinEventDelay").DocumentationString =
                        "Sets the minimum time between events being handled";
                    GetLibraryFunctionSignature("llSoundPreload").DocumentationString =
                        "Preloads a sound on viewers within range";
                    GetLibraryFunctionSignature("llRotLookAt").DocumentationString =
                        "Causes object to point its forward axis towards target";
                    GetLibraryFunctionSignature("llStringLength").DocumentationString =
                        "Returns the length of string";
                    GetLibraryFunctionSignature("llStartAnimation").DocumentationString =
                        "Starts animation anim for agent that granted PERMISSION_TRIGGER_ANIMATION if the permission has not been revoked"
                            ;
                    GetLibraryFunctionSignature("llStopAnimation").DocumentationString =
                        "Stops animation anim for agent that granted permission";
                    GetLibraryFunctionSignature("llPointAt").DocumentationString =
                        "Makes agent that owns object point at pos";
                    GetLibraryFunctionSignature("llStopPointAt").DocumentationString =
                        "Stops pointing agent that owns object";
                    GetLibraryFunctionSignature("llTargetOmega").DocumentationString =
                        "Rotates the object around axis at spinrate with strength gain";
                    GetLibraryFunctionSignature("llGetStartParameter").DocumentationString =
                        "Returns an integer that is the script start/rez parameter";
                    GetLibraryFunctionSignature("llGodLikeRezObject").DocumentationString =
                        "Rezzes directly off of UUID if owner is in God Mode";
                    GetLibraryFunctionSignature("llRequestPermissions").DocumentationString =
                        "Asks the agent for permission to run certain classes of functions";
                    GetLibraryFunctionSignature("llGetPermissionsKey").DocumentationString =
                        "Returns the key of the avatar that last granted permissions to the script";
                    GetLibraryFunctionSignature("llGetPermissions").DocumentationString =
                        "Returns an integer bitfield with the permissions that have been granted";
                    GetLibraryFunctionSignature("llGetLinkNumber").DocumentationString =
                        "Returns the link number of the prim containing the script (0 means not linked, 1 the prim is the root, 2 the prim is the first child, etc)"
                            ;
                    GetLibraryFunctionSignature("llSetLinkColor").DocumentationString =
                        "Sets face to color if a task exists in the link chain at linknumber";
                    GetLibraryFunctionSignature("llCreateLink").DocumentationString =
                        "Attempts to link the script&apos;s object with the target (requires that PERMISSION_CHANGE_LINKS be granted). If parent == TRUE, then the script&apos;s object becomes the root"
                            ;
                    GetLibraryFunctionSignature("llBreakLink").DocumentationString =
                        "Delinks the prim with the given link number in a linked object set (requires that PERMISSION_CHANGE_LINKS be granted)"
                            ;
                    GetLibraryFunctionSignature("llBreakAllLinks").DocumentationString =
                        "Delinks all prims in the link set (requires that PERMISSION_CHANGE_LINKS be granted)"
                            ;
                    GetLibraryFunctionSignature("llGetLinkKey").DocumentationString =
                        "Returns the key of the linked prim linknumber";
                    GetLibraryFunctionSignature("llGetLinkName").DocumentationString =
                        "Returns the name of linknumber in a link set";
                    GetLibraryFunctionSignature("llGetInventoryNumber").DocumentationString =
                        "Returns the number of items of a given type (INVENTORY_* flag) in the prim&apos;s inventory"
                            ;
                    GetLibraryFunctionSignature("llGetInventoryName").DocumentationString =
                        "Returns the name of the inventory item number of a given type";
                    GetLibraryFunctionSignature("llSetScriptState").DocumentationString =
                        "Sets the running state of the specified script";
                    GetLibraryFunctionSignature("llGetEnergy").DocumentationString =
                        "Returns how much energy is in the object as a percentage of maximum";
                    GetLibraryFunctionSignature("llGiveInventory").DocumentationString =
                        "Gives inventory to destination";
                    GetLibraryFunctionSignature("llRemoveInventory").DocumentationString =
                        "Removes the named inventory item";
                    GetLibraryFunctionSignature("llSetText").DocumentationString =
                        "Displays text that hovers over the prim with specific color and translucency specified with alpha"
                            ;
                    GetLibraryFunctionSignature("llPassTouches").DocumentationString =
                        "If pass == TRUE, touches are passed from children on to parents";
                    GetLibraryFunctionSignature("llRequestAgentData").DocumentationString =
                        "Requests data about agent id. When data is available the dataserver event will be raised"
                            ;
                    GetLibraryFunctionSignature("llRequestInventoryData").DocumentationString =
                        "Requests data from object&apos;s inventory object. When data is available the dataserver event will be raised"
                            ;
                    GetLibraryFunctionSignature("llSetDamage").DocumentationString =
                        "Sets the amount of damage that will be done when this object hits an avatar";
                    GetLibraryFunctionSignature("llTeleportAgentHome").DocumentationString =
                        "Teleports avatar on the owner&apos;s land to their home location without any warning"
                            ;
                    GetLibraryFunctionSignature("llModifyLand").DocumentationString =
                        "Modifies land using the specified action on the specified brush size of land";
                    GetLibraryFunctionSignature("llCollisionSound").DocumentationString =
                        "Suppresses default collision sounds, replaces default impact sounds with impact_sound at the volume impact_volume"
                            ;
                    GetLibraryFunctionSignature("llCollisionSprite").DocumentationString =
                        "Suppresses default collision sprites, replaces default impact sprite with impact_sprite (use an empty string to just suppress)"
                            ;
                    GetLibraryFunctionSignature("llGetAnimation").DocumentationString =
                        "Returns the name of the currently playing locomotion animation for avatar id";
                    GetLibraryFunctionSignature("llResetScript").DocumentationString = "Resets the script";
                    GetLibraryFunctionSignature("llMessageLinked").DocumentationString =
                        "Allows scripts in the same object to communicate. Triggers a link_message event with the same parameters num, str, and id in all scripts in the prim(s) described by linknum."
                            ;
                    GetLibraryFunctionSignature("llPushObject").DocumentationString =
                        "Applies impulse and ang_impulse to object id";
                    GetLibraryFunctionSignature("llPassCollisions").DocumentationString =
                        "If pass == TRUE, collisions are passed from children on to parents (default is FALSE)"
                            ;
                    GetLibraryFunctionSignature("llGetScriptName").DocumentationString =
                        "Returns the name of the script that this function is used in";
                    GetLibraryFunctionSignature("llGetNumberOfSides").DocumentationString =
                        "Returns the number of faces (or sides) of the prim";
                    GetLibraryFunctionSignature("llGetInventoryKey").DocumentationString =
                        "Returns the key that is the UUID of the inventory name";
                    GetLibraryFunctionSignature("llAllowInventoryDrop").DocumentationString =
                        "If add == TRUE, users without modify permissions can still drop inventory items onto a prim"
                            ;
                    GetLibraryFunctionSignature("llGetSunDirection").DocumentationString =
                        "Returns a normalized vector of the direction of the sun in the region";
                    GetLibraryFunctionSignature("llGetTextureOffset").DocumentationString =
                        "Returns the texture offset of face in the x and y components of a vector";
                    GetLibraryFunctionSignature("llGetTextureScale").DocumentationString =
                        "Returns the texture scale of side in the x and y components of a vector";
                    GetLibraryFunctionSignature("llGetTextureRot").DocumentationString =
                        "Returns the texture rotation of side";
                    GetLibraryFunctionSignature("llSubStringIndex").DocumentationString =
                        "Returns an integer that is the index in source where pattern first appears.;(Returns -1 if not found)"
                            ;
                    GetLibraryFunctionSignature("llGetOwnerKey").DocumentationString =
                        "Returns the owner of object id";
                    GetLibraryFunctionSignature("llGetCenterOfMass").DocumentationString =
                        "Returns the prim&apos;s center of mass (unless called from the root prim, where it returns the object&apos;s center of mass)"
                            ;
                    GetLibraryFunctionSignature("llListSort").DocumentationString =
                        "Sorts the list into blocks of stride, in ascending order if ascending == TRUE.;The sort order is affected by type"
                            ;
                    GetLibraryFunctionSignature("llGetListLength").DocumentationString =
                        "Returns the number of elements in the list";
                    GetLibraryFunctionSignature("llList2Integer").DocumentationString =
                        "Copies the integer at index in the list";
                    GetLibraryFunctionSignature("llList2Float").DocumentationString =
                        "Copies the float at index in the list";
                    GetLibraryFunctionSignature("llList2String").DocumentationString =
                        "Copies the string at index in the list";
                    GetLibraryFunctionSignature("llList2Key").DocumentationString =
                        "Copies the key at index in the list";
                    GetLibraryFunctionSignature("llList2Vector").DocumentationString =
                        "Copies the vector at index in the list";
                    GetLibraryFunctionSignature("llList2Rot").DocumentationString =
                        "Copies the rotation at index in the list";
                    GetLibraryFunctionSignature("llList2List").DocumentationString =
                        "Copies the slice of the list from start to end";
                    GetLibraryFunctionSignature("llDeleteSubList").DocumentationString =
                        "Removes the slice from start to end and returns the remainder of the list";
                    GetLibraryFunctionSignature("llGetListEntryType").DocumentationString =
                        "Returns the type of the index entry in the list;(TYPE_INTEGER, TYPE_FLOAT, TYPE_STRING, TYPE_KEY, TYPE_VECTOR, TYPE_ROTATION, or TYPE_INVALID if index is off list)"
                            ;
                    GetLibraryFunctionSignature("llList2CSV").DocumentationString =
                        "Creates a string of comma separated values from list";
                    GetLibraryFunctionSignature("llCSV2List").DocumentationString =
                        "Creates a list from a string of comma separated values";
                    GetLibraryFunctionSignature("llListRandomize").DocumentationString =
                        "Returns a randomized list of blocks of size stride";
                    GetLibraryFunctionSignature("llList2ListStrided").DocumentationString =
                        "Copies the strided slice of the list from start to end";
                    GetLibraryFunctionSignature("llGetRegionCorner").DocumentationString =
                        "Returns a vector in meters that is the global location of the south-west corner of the region which the object is in"
                            ;
                    GetLibraryFunctionSignature("llListInsertList").DocumentationString =
                        "Returns a list that contains all the elements from dest but with the elements from src inserted at position start"
                            ;
                    GetLibraryFunctionSignature("llListFindList").DocumentationString =
                        "Returns the index of the first instance of test in src";
                    GetLibraryFunctionSignature("llGetObjectName").DocumentationString =
                        "Returns the name of the prim which the script is attached to";
                    GetLibraryFunctionSignature("llSetObjectName").DocumentationString =
                        "Sets the prim&apos;s name to the name parameter";
                    GetLibraryFunctionSignature("llGetDate").DocumentationString =
                        "Returns the current date in the UTC time zone in the format YYYY-MM-DD";
                    GetLibraryFunctionSignature("llEdgeOfWorld").DocumentationString =
                        "Checks to see whether the border hit by dir from pos is the edge of the world (has no neighboring region)"
                            ;
                    GetLibraryFunctionSignature("llGetAgentInfo").DocumentationString =
                        "Returns an integer bitfield containing the agent information about id.;Returns AGENT_FLYING, AGENT_ATTACHMENTS, AGENT_SCRIPTED, AGENT_SITTING, AGENT_ON_OBJECT, AGENT_MOUSELOOK, AGENT_AWAY, AGENT_BUSY, AGENT_TYPING, AGENT_CROUCHING, AGENT_ALWAYS_RUN, AGENT_WALKING and/or AGENT_IN_AIR"
                            ;
                    GetLibraryFunctionSignature("llAdjustSoundVolume").DocumentationString =
                        "Adjusts volume of attached sound (0.0 - 1.0)";
                    GetLibraryFunctionSignature("llSetSoundQueueing").DocumentationString =
                        "Sets whether attached sounds wait for the current sound to finish (If queue == TRUE then queuing is enabled, if FALSE queuing is disabled [default])"
                            ;
                    GetLibraryFunctionSignature("llSetSoundRadius").DocumentationString =
                        "Establishes a hard cut-off radius for audibility of scripted sounds (both attached and triggered)"
                            ;
                    GetLibraryFunctionSignature("llKey2Name").DocumentationString =
                        "Returns the name of the prim or avatar specified by id.;(The id must be a valid rezzed prim or avatar key in the current simulator, otherwise an empty string is returned.)"
                            ;
                    GetLibraryFunctionSignature("llSetTextureAnim").DocumentationString =
                        "Animates the texture on the specified face/faces";
                    GetLibraryFunctionSignature("llTriggerSoundLimited").DocumentationString =
                        "Plays sound at volume (0.0 - 1.0), centered at but not attached to object, limited to the box defined by vectors top_north_east and bottom_south_west"
                            ;
                    GetLibraryFunctionSignature("llEjectFromLand").DocumentationString =
                        "Ejects avatar from the parcel";
                    GetLibraryFunctionSignature("llParseString2List").DocumentationString =
                        "Breaks src into a list, discarding separators, keeping spacers;(separators and spacers must be lists of strings, maximum of 8 each)"
                            ;
                    GetLibraryFunctionSignature("llOverMyLand").DocumentationString =
                        "Returns TRUE if id is over land owned by the script owner, otherwise FALSE";
                    GetLibraryFunctionSignature("llGetLandOwnerAt").DocumentationString =
                        "Returns the key of the land owner, returns NULL_KEY if public";
                    GetLibraryFunctionSignature("llGetNotecardLine").DocumentationString =
                        "Returns line line of notecard name via the dataserver event";
                    GetLibraryFunctionSignature("llGetAgentSize").DocumentationString =
                        "If the avatar is in the same region, returns the size of the bounding box of the requested avatar by id, otherwise returns ZERO_VECTOR"
                            ;
                    GetLibraryFunctionSignature("llSameGroup").DocumentationString =
                        "Returns TRUE if avatar id is in the same region and has the same active group, otherwise FALSE"
                            ;
                    GetLibraryFunctionSignature("llUnSit").DocumentationString =
                        "If avatar identified by id is sitting on the object the script is attached to or is over land owned by the object&apos;s owner, the avatar is forced to stand up"
                            ;
                    GetLibraryFunctionSignature("llGroundSlope").DocumentationString =
                        "Returns the ground slope below the object position + offset";
                    GetLibraryFunctionSignature("llGroundNormal").DocumentationString =
                        "Returns the ground normal below the object position + offset";
                    GetLibraryFunctionSignature("llGroundContour").DocumentationString =
                        "Returns the ground contour direction below the object position + offset";
                    GetLibraryFunctionSignature("llGetAttached").DocumentationString =
                        "Returns the object&apos;s attachment point, or 0 if not attached";
                    GetLibraryFunctionSignature("llGetFreeMemory").DocumentationString =
                        "Returns the number of free bytes of memory the script can use";
                    GetLibraryFunctionSignature("llGetRegionName").DocumentationString =
                        "Returns the current region name";
                    GetLibraryFunctionSignature("llGetRegionTimeDilation").DocumentationString =
                        "Returns the current time dilation as a float between 0.0 (full dilation) and 1.0 (no dilation)"
                            ;
                    GetLibraryFunctionSignature("llGetRegionFPS").DocumentationString =
                        "Returns the mean region frames per second";
                    GetLibraryFunctionSignature("llParticleSystem").DocumentationString =
                        "Creates a particle system based on rules.  An empty list removes the particle system.;List format is [ rule1, data1, rule2, data2 . . . rulen, datan ]"
                            ;
                    GetLibraryFunctionSignature("llGroundRepel").DocumentationString =
                        "Critically damps to height if within height*0.5 of level (either above ground level, or above the higher of land and water if water == TRUE)"
                            ;
                    GetLibraryFunctionSignature("llGiveInventoryList").DocumentationString =
                        "Gives inventory items to target, creating a new folder to put them in";
                    GetLibraryFunctionSignature("llSetVehicleType").DocumentationString =
                        "Sets the vehicle to one of the default types";
                    GetLibraryFunctionSignature("llSetVehicleFloatParam").DocumentationString =
                        "Sets the specified vehicle float parameter";
                    GetLibraryFunctionSignature("llSetVehicleVectorParam").DocumentationString =
                        "Sets the specified vehicle vector parameter";
                    GetLibraryFunctionSignature("llSetVehicleRotationParam").DocumentationString =
                        "Sets the specified vehicle rotation parameter";
                    GetLibraryFunctionSignature("llSetVehicleFlags").DocumentationString =
                        "Sets the enabled bits in &quot;flags&quot;";
                    GetLibraryFunctionSignature("llRemoveVehicleFlags").DocumentationString =
                        "Removes the enabled bits in &apos;flags&apos;";
                    GetLibraryFunctionSignature("llSitTarget").DocumentationString =
                        "Sets the sit location for the prim.  If offset == &lt;0,0,0&gt; then the sit target is removed."
                            ;
                    GetLibraryFunctionSignature("llAvatarOnSitTarget").DocumentationString =
                        "If an avatar is seated on the sit target, returns the avatar&apos;s key, otherwise NULL_KEY"
                            ;
                    GetLibraryFunctionSignature("llAddToLandPassList").DocumentationString =
                        "Adds avatar to the land pass list for hours, or indefinitely if hours is 0";
                    GetLibraryFunctionSignature("llSetTouchText").DocumentationString =
                        "Displays text rather than the default &apos;Touch&apos; in the pie menu";
                    GetLibraryFunctionSignature("llSetSitText").DocumentationString =
                        "Displays text rather than the default &apos;Sit Here&apos; in the pie menu";
                    GetLibraryFunctionSignature("llSetCameraEyeOffset").DocumentationString =
                        "Sets the camera eye offset for avatars that sit on the object";
                    GetLibraryFunctionSignature("llSetCameraAtOffset").DocumentationString =
                        "Sets the point the camera is looking at to offset for avatars that sit on the object"
                            ;
                    GetLibraryFunctionSignature("llDumpList2String").DocumentationString =
                        "Returns the list in a single string, using separator between the entries";
                    GetLibraryFunctionSignature("llScriptDanger").DocumentationString =
                        "Returns TRUE if pos is over public land, sandbox land, land that doesn&apos;t allow everyone to edit and build, or land that doesn&apos;t allow outside scripts"
                            ;
                    GetLibraryFunctionSignature("llDialog").DocumentationString =
                        "Shows a dialog box on the avatar&apos;s screen with a message and up to 12 buttons.;If a button is pressed, the avatar says the text of the button label on chat_channel"
                            ;
                    GetLibraryFunctionSignature("llVolumeDetect").DocumentationString =
                        "If detect = TRUE, object works much like Phantom, but triggers collision_start and collision_end events when other objects start and stop interpenetrating.;Must be applied to the root prim."
                            ;
                    GetLibraryFunctionSignature("llResetOtherScript").DocumentationString =
                        "Resets script name";
                    GetLibraryFunctionSignature("llGetScriptState").DocumentationString =
                        "Returns TRUE if the script name is running";
                    GetLibraryFunctionSignature("llRemoteLoadScript").DocumentationString =
                        "DEPRECATED!  Please use llRemoteLoadScriptPin instead";
                    GetLibraryFunctionSignature("llSetRemoteScriptAccessPin").DocumentationString =
                        "If pin is set to a non-zero number, allows a prim to have scripts remotely loaded via llRemoteLoadScriptPin when it passes in the correct pin. Otherwise, llRemoteLoadScriptPin is ignored"
                            ;
                    GetLibraryFunctionSignature("llRemoteLoadScriptPin").DocumentationString =
                        "Copies script name onto target, if the owner of this scripted object can modify target and is in the same region, and the matching pin is used.;If running == TRUE, starts the script with start_param"
                            ;
                    GetLibraryFunctionSignature("llOpenRemoteDataChannel").DocumentationString =
                        "Creates a channel to listen for XML-RPC calls, and will trigger a remote_data event with channel id once it is available"
                            ;
                    GetLibraryFunctionSignature("llSendRemoteData").DocumentationString =
                        "Sends an XML-RPC request to dest through channel with payload of channel (in a string), integer idata and string sdata.;Returns a key that is the message_id for the resulting remote_data events."
                            ;
                    GetLibraryFunctionSignature("llRemoteDataReply").DocumentationString =
                        "Sends an XML-RPC reply to message_id on channel with payload of string sdata and integer idata"
                            ;
                    GetLibraryFunctionSignature("llCloseRemoteDataChannel").DocumentationString =
                        "Closes XML-RPC channel";
                    GetLibraryFunctionSignature("llMD5String").DocumentationString =
                        "Returns a string of 32 hex characters that is a RSA Data Security, Inc. MD5 Message-Digest Algorithm of src with nonce"
                            ;
                    GetLibraryFunctionSignature("llSetPrimitiveParams").DocumentationString =
                        "Sets the prim&apos;s parameters according to rules";
                    GetLibraryFunctionSignature("llStringToBase64").DocumentationString =
                        "Converts a string to the Base64 representation of the string";
                    GetLibraryFunctionSignature("llBase64ToString").DocumentationString =
                        "Converts a Base64 string to a conventional string.;If the conversion creates any unprintable characters, they are converted to spaces"
                            ;
                    GetLibraryFunctionSignature("llIntegerToBase64").DocumentationString =
                        " Returns a string that is a Base64 big endian encode of number";
                    GetLibraryFunctionSignature("llBase64ToInteger").DocumentationString =
                        "Returns an integer that is the str Base64 decoded as a big endian integer";
                    GetLibraryFunctionSignature("llRemoteDataSetRegion").DocumentationString =
                        "DEPRECATED!  Please use llOpenRemoteDataChannel instead.;If an object using remote data channels changes regions, you must call this function to reregister the remote data channels. This call is not needed if the prim does not change regions"
                            ;
                    GetLibraryFunctionSignature("llLog10").DocumentationString =
                        "Returns the base 10 logarithm of val.  Returns zero if val &lt;= 0";
                    GetLibraryFunctionSignature("llLog").DocumentationString =
                        "Returns the natural logarithm of val.  Returns zero if val &lt;= 0";
                    GetLibraryFunctionSignature("llGetAnimationList").DocumentationString =
                        "Returns a list of keys of playing animations for avatar described by id";
                    GetLibraryFunctionSignature("llSetParcelMusicURL").DocumentationString =
                        "Sets the streaming audio URL for the parcel which the object is on";
                    GetLibraryFunctionSignature("llGetRootPosition").DocumentationString =
                        "Returns the position (in region coordinates) of the root prim of the object which the script is attached to"
                            ;
                    GetLibraryFunctionSignature("llGetRootRotation").DocumentationString =
                        "Returns the rotation (relative to the region) of the root prim of the object which the script is attached to"
                            ;
                    GetLibraryFunctionSignature("llGetObjectDesc").DocumentationString =
                        "Returns the description of the prim the script is attached to";
                    GetLibraryFunctionSignature("llSetObjectDesc").DocumentationString =
                        "Sets the prim&apos;s description";
                    GetLibraryFunctionSignature("llGetCreator").DocumentationString =
                        "Returns a key for the creator of the prim";
                    GetLibraryFunctionSignature("llGetTimestamp").DocumentationString =
                        "Returns the timestamp in the UTC time zone in the format; YYYY-MM-DDThh;mm;ss.ff..fZ"
                            ;
                    GetLibraryFunctionSignature("llGetGMTclock").DocumentationString =
                        "Returns the time in seconds since midnight GMT";
                    GetLibraryFunctionSignature("llSetLinkAlpha").DocumentationString =
                        "If a prim exists in the link chain at linknumber, sets face to alpha";
                    GetLibraryFunctionSignature("llGetNumberOfPrims").DocumentationString =
                        "Returns the number of prims in a link set the script is attached to";
                    GetLibraryFunctionSignature("llGetNumberOfNotecardLines").DocumentationString =
                        "Returns number of lines in notecard name via the dataserver event (cast return value to integer)"
                            ;
                    GetLibraryFunctionSignature("llGetBoundingBox").DocumentationString =
                        "Returns the bounding box around the object (including any linked prims) relative to its root prim, in a list in the format [ (vector) min_corner, (vector) max_corner ]"
                            ;
                    GetLibraryFunctionSignature("llGetGeometricCenter").DocumentationString =
                        "Returns the geometric center of the linked set the script is attached to";
                    GetLibraryFunctionSignature("llGetPrimitiveParams").DocumentationString =
                        "Returns the primitive parameters specified in the params list";
                    GetLibraryFunctionSignature("llGetSimulatorHostname").DocumentationString =
                        "Returns the hostname of the machine which the script is running on (same as string in viewer Help dialog)"
                            ;
                    GetLibraryFunctionSignature("llSetLocalRot").DocumentationString =
                        "Sets the rotation of a child prim relative to the root prim";
                    GetLibraryFunctionSignature("llParseStringKeepNulls").DocumentationString =
                        "Breaks src into a list, discarding separators, keeping spacers, keeping any null values generated.;(separators and spacers must be lists of strings, maximum of 8 each)"
                            ;
                    GetLibraryFunctionSignature("llRezAtRoot").DocumentationString =
                        "Instantiates owner&apos;s inventory object rotated to rot with its root at pos, moving at vel, using param as the start parameter"
                            ;
                    GetLibraryFunctionSignature("llGetObjectPermMask").DocumentationString =
                        "Returns the requested permission mask for the root object the task is attached to";
                    GetLibraryFunctionSignature("llSetObjectPermMask").DocumentationString =
                        "Sets the given permission mask to the new value on the root object the task is attached to (requires God Mode)"
                            ;
                    GetLibraryFunctionSignature("llGetInventoryPermMask").DocumentationString =
                        "Returns the requested permission mask for the inventory item";
                    GetLibraryFunctionSignature("llSetInventoryPermMask").DocumentationString =
                        "Sets the given permission mask to the new value on the inventory item (requires God Mode)"
                            ;
                    GetLibraryFunctionSignature("llGetInventoryCreator").DocumentationString =
                        "Returns a key for the creator of the inventory item";
                    GetLibraryFunctionSignature("llOwnerSay").DocumentationString =
                        "Says message to owner only.  (Owner must be in the same region.)";
                    GetLibraryFunctionSignature("llRequestSimulatorData").DocumentationString =
                        "Requests data about simulator.  When data is available the dataserver event will be raised"
                            ;
                    GetLibraryFunctionSignature("llForceMouselook").DocumentationString =
                        "If mouselook is TRUE, any avatar that sits upon the prim will be forced into mouselook mode"
                            ;
                    GetLibraryFunctionSignature("llGetObjectMass").DocumentationString =
                        "Returns the mass of the avatar or object in the region";
                    GetLibraryFunctionSignature("llListReplaceList").DocumentationString =
                        "Returns a list that is dest with start through end removed and src inserted at start"
                            ;
                    GetLibraryFunctionSignature("llLoadURL").DocumentationString =
                        "Shows a dialog to avatar offering to load the web page at url with a message.;If user clicks yes, launches the page in their web browser"
                            ;
                    GetLibraryFunctionSignature("llParcelMediaCommandList").DocumentationString =
                        "Sends a list of commands, some with arguments, to a parcel to control the playback of movies and other media"
                            ;
                    GetLibraryFunctionSignature("llParcelMediaQuery").DocumentationString =
                        "Returns a list containing results of the sent query";
                    GetLibraryFunctionSignature("llGetInventoryType").DocumentationString =
                        "Returns the type of the inventory item name";
                    GetLibraryFunctionSignature("llSetPayPrice").DocumentationString =
                        "Sets the default amount on the dialog that appears when someone chooses to pay this prim"
                            ;
                    GetLibraryFunctionSignature("llGetCameraPos").DocumentationString =
                        "Returns the current camera position for the agent the task has permissions for";
                    GetLibraryFunctionSignature("llGetCameraRot").DocumentationString =
                        "Returns the current camera orientation for the agent the task has permissions for";
                    GetLibraryFunctionSignature("llSetPrimURL").DocumentationString =
                        "Updates the URL for the web page shown on the sides of the object";
                    GetLibraryFunctionSignature("llRefreshPrimURL").DocumentationString =
                        "Reloads the web page shown on the sides of the object";
                    GetLibraryFunctionSignature("llEscapeURL").DocumentationString =
                        "Returns an escaped/encoded version of url, replacing spaces with %20 etc";
                    GetLibraryFunctionSignature("llUnescapeURL").DocumentationString =
                        "Returns an unescaped/ unencoded version of url, replacing %20 with spaces etc";
                    GetLibraryFunctionSignature("llMapDestination").DocumentationString =
                        "Opens the World Map centered on the region simname with pos highlighted. (NOTE; look_at currently does nothing.);Only works for scripts attached to avatar, or during touch events"
                            ;
                    GetLibraryFunctionSignature("llAddToLandBanList").DocumentationString =
                        "Add avatar to the land ban list for hours, or indefinitely if hours is zero";
                    GetLibraryFunctionSignature("llRemoveFromLandPassList").DocumentationString =
                        "Removes avatar from the land pass list";
                    GetLibraryFunctionSignature("llRemoveFromLandBanList").DocumentationString =
                        "Removes avatar from the land ban list";
                    GetLibraryFunctionSignature("llSetCameraParams").DocumentationString =
                        "Sets multiple camera parameters at once.;List format is [ rule1, data1, rule2, data2 . . . rulen, datan ]"
                            ;
                    GetLibraryFunctionSignature("llClearCameraParams").DocumentationString =
                        "Resets all camera parameters to default values and turns off scripted camera control"
                            ;
                    GetLibraryFunctionSignature("llListStatistics").DocumentationString =
                        "Performs statistical aggregate functions on list src using LIST_STAT_* operations";
                    GetLibraryFunctionSignature("llGetParcelFlags").DocumentationString =
                        "Returns a mask of the parcel flags (PARCEL_FLAG_*) for the parcel that includes the point pos"
                            ;
                    GetLibraryFunctionSignature("llGetRegionFlags").DocumentationString =
                        "Returns the region flags (REGION_FLAG_*) for the region the object is in";
                    GetLibraryFunctionSignature("llXorBase64StringsCorrect").DocumentationString =
                        "Correctly performs an exclusive or on two Base64 strings.;s2 repeats if it is shorter than s1"
                            ;
                    GetLibraryFunctionSignature("llXorBase64").DocumentationString =
                        "Correctly performs an exclusive or on two Base64 strings and returns a Base64 string.;If the inputs are not Base64 strings the result will be erratic"
                            ;
                    GetLibraryFunctionSignature("llHTTPRequest").DocumentationString =
                        "Sends an HTTP request to the specified url with the body of the request and parameters"
                            ;
                    GetLibraryFunctionSignature("llResetLandBanList").DocumentationString =
                        "Removes all Residents from the land ban list";
                    GetLibraryFunctionSignature("llResetLandPassList").DocumentationString =
                        "Removes all Residents from the land access/pass list";
                    GetLibraryFunctionSignature("llGetObjectPrimCount").DocumentationString =
                        "Returns the total number of prims for an object in the region";
                    GetLibraryFunctionSignature("llGetParcelPrimOwners").DocumentationString =
                        "Returns a list of all Residents who own objects on the parcel at pos and with individual prim counts.;Requires owner-like permissions for the parcel"
                            ;
                    GetLibraryFunctionSignature("llGetParcelPrimCount").DocumentationString =
                        "Returns the number of prims on the parcel at pos of the given category.;Categories; PARCEL_COUNT_TOTAL, _OWNER, _GROUP, _OTHER, _SELECTED, _TEMP"
                            ;
                    GetLibraryFunctionSignature("llGetParcelMaxPrims").DocumentationString =
                        "Returns the maximum number of prims allowed on the parcel at pos";
                    GetLibraryFunctionSignature("llGetParcelDetails").DocumentationString =
                        "Returns the parcel details specified in params for the parcel at pos.;Params is one or more of; PARCEL_DETAILS_NAME, _DESC, _OWNER, _GROUP, _AREA, _ID, _SEE_AVATARS"
                            ;
                    GetLibraryFunctionSignature("llSetLinkPrimitiveParams").DocumentationString =
                        "Sets primitive parameters for linknumber based on rules";
                    GetLibraryFunctionSignature("llSetLinkTexture").DocumentationString =
                        "Sets the texture of face for a task that exists in the link chain at linknumber";
                    GetLibraryFunctionSignature("llStringTrim").DocumentationString =
                        "Trims the leading and/or trailing white spaces from a string.;trim_type can be STRING_TRIM, STRING_TRIM_HEAD or STRING_TRIM_TAIL"
                            ;
                    GetLibraryFunctionSignature("llRegionSay").DocumentationString =
                        "Broadcasts message on channel (not 0) that can be heard anywhere in the region by a script listening on channel"
                            ;
                    GetLibraryFunctionSignature("llGetObjectDetails").DocumentationString =
                        "Returns the object details specified in params for the object with key id.;Params are OBJECT_NAME, _DESC, _POS, _ROT, _VELOCITY, _OWNER, _GROUP, _CREATOR"
                            ;
                    GetLibraryFunctionSignature("llSetClickAction").DocumentationString =
                        "Sets the action performed when a prim is clicked upon";
                    GetLibraryFunctionSignature("llGetRegionAgentCount").DocumentationString =
                        "Returns the number of avatars in the region";
                    GetLibraryFunctionSignature("llTextBox").DocumentationString =
                        "Shows a window on the avatar&apos;s screen with the message.;It contains a text box for input, and if entered that text is chatted on chat_channel"
                            ;
                    GetLibraryFunctionSignature("llGetAgentLanguage").DocumentationString =
                        "Returns the language code of the preferred interface language of the avatar";
                    GetLibraryFunctionSignature("llDetectedTouchUV").DocumentationString =
                        "Returns the u and v coordinates in the first two components of a vector, for the texture coordinates where the prim was touched in a triggered touch event"
                            ;
                    GetLibraryFunctionSignature("llDetectedTouchFace").DocumentationString =
                        "Returns the index of the face where the avatar clicked in a triggered touch event";
                    GetLibraryFunctionSignature("llDetectedTouchPos").DocumentationString =
                        "Returns the position where the object was touched in a triggered touch event";
                    GetLibraryFunctionSignature("llDetectedTouchNormal").DocumentationString =
                        "Returns the surface normal for a triggered touch event";
                    GetLibraryFunctionSignature("llDetectedTouchBinormal").DocumentationString =
                        "Returns the surface binormal for a triggered touch event";
                    GetLibraryFunctionSignature("llDetectedTouchST").DocumentationString =
                        "Returns the s and t coordinates in the first two components of a vector, for the surface coordinates where the prim was touched in a triggered touch event"
                            ;
                    GetLibraryFunctionSignature("llSHA1String").DocumentationString =
                        "Returns a string of 40 hex characters that is the SHA1 security Hash of src";
                    GetLibraryFunctionSignature("llGetFreeURLs").DocumentationString =
                        "Returns the number of available URLs for the current script";
                    GetLibraryFunctionSignature("llRequestURL").DocumentationString =
                        "Requests one HTTP;// url for use by this object.;An http_request event is triggered with the results"
                            ;
                    GetLibraryFunctionSignature("llRequestSecureURL").DocumentationString =
                        "Requests one HTTPS;// (SSL) url for use by this object.;An http_request event is triggered with the results"
                            ;
                    GetLibraryFunctionSignature("llReleaseURL").DocumentationString =
                        "Releases the specified URL, it will no longer be usable";
                    GetLibraryFunctionSignature("llHTTPResponse").DocumentationString =
                        "Responds to request_id with status and body";
                    GetLibraryFunctionSignature("llGetHTTPHeader").DocumentationString =
                        "Returns the value for header for request_id";
                    GetLibraryFunctionSignature("llSetPrimMediaParams").DocumentationString =
                        "Sets the media params for a particular face on an object. If media is not already on this object, add it.;List is a set of name/value pairs in no particular order.  Params not specified are unchanged, or if new media is added then set to the default specified.;The possible names are below, along with the types of values and what they mean"
                            ;
                    GetLibraryFunctionSignature("llGetPrimMediaParams").DocumentationString =
                        "Returns the media params for a particular face on an object, given the desired list of names, in the order requested.;(Returns an empty list if no media exists on the face.)"
                            ;
                    GetLibraryFunctionSignature("llClearPrimMedia").DocumentationString =
                        "Clears (deletes) the media and all params from the given face";
                    GetLibraryFunctionSignature("llSetLinkPrimitiveParamsFast").DocumentationString =
                        "Set primitive parameters for linknumber based on rules";
                    GetLibraryFunctionSignature("llGetLinkPrimitiveParams").DocumentationString =
                        "Get primitive parameters for linknumber based on rules";
                    GetLibraryFunctionSignature("llLinkParticleSystem").DocumentationString =
                        "Creates a particle system based on rules.  Empty list removes particle system from object.;List format is [ rule1, data1, rule2, data2 . . . rulen, datan ]"
                            ;
                    GetLibraryFunctionSignature("llSetLinkTextureAnim").DocumentationString =
                        "Animate the texture on the specified prim&apos;s face/faces";
                    GetLibraryFunctionSignature("llGetLinkNumberOfSides").DocumentationString =
                        "Returns the number of sides of the specified linked prim";
                    GetLibraryFunctionSignature("llGetUsername").DocumentationString =
                        "Returns the single-word username of an avatar, iff the avatar is in the current region, otherwise the empty string"
                            ;
                    GetLibraryFunctionSignature("llRequestUsername").DocumentationString =
                        "Requests single-word username of an avatar.  When data is available the dataserver event will be raised"
                            ;
                    GetLibraryFunctionSignature("llGetDisplayName").DocumentationString =
                        "Returns the name of an avatar, iff the avatar is in the current simulator, and the name has been cached, otherwise the same as llGetUsername.  Use llRequestDisplayName if you absolutely must have the display name"
                            ;
                    GetLibraryFunctionSignature("llRequestDisplayName").DocumentationString =
                        "Requests name of an avatar.  When data is available the dataserver event will be raised"
                            ;
                    GetLibraryFunctionSignature("llGetEnv").DocumentationString =
                        "Returns a string with the requested data about the region";
                    GetLibraryFunctionSignature("llCastRay").DocumentationString =
                        "Casts a ray into the physics world from &apos;start&apos; to &apos;end&apos; and returns data according to details in params"
                            ;
                    GetLibraryFunctionSignature("llRegionSayTo").DocumentationString =
                        "Sends message on channel (not DEBUG_CHANNEL) directly to prim or avatar target anywhere within the region"
                            ;
                    GetLibraryFunctionSignature("llGetSPMaxMemory").DocumentationString =
                        "Returns the maximum used memory for the current script. Only valid after using PROFILE_SCRIPT_MEMORY. Non-mono scripts always use 16k"
                            ;
                    GetLibraryFunctionSignature("llGetUsedMemory").DocumentationString =
                        "Returns the current used memory for the current script. Non-mono scripts always use 16k"
                            ;
                    GetLibraryFunctionSignature("llScriptProfiler").DocumentationString =
                        "Enabled or disables script profiling options. Currently only supports PROFILE_SCRIPT_MEMORY (mono only) and PROFILE_NONE.  MAY SIGNIFICANTLY REDUCE SCRIPT PERFORMANCE!"
                            ;
                    GetLibraryFunctionSignature("llSetMemoryLimit").DocumentationString =
                        "Request limit bytes to be reserved for this script.;Returns a success/failure flag (STATUS_OK when successful, another of the STATUS_* flags on failure) for whether the memory limit was set.;Only relevant for Mono-compiled scripts"
                            ;
                    GetLibraryFunctionSignature("llGetMemoryLimit").DocumentationString =
                        "Get the maximum memory a script can use.;Returns the integer amount of memory the script can use in bytes"
                            ;
                    GetLibraryFunctionSignature("llSetLinkMedia").DocumentationString =
                        "Set the media params for a particular face on linked prim.  List is a set of name/value pairs (in no particular order). The possible names are below, along with the types of values and what they mean.  If media is not already on this object, add it. Params not specified are unchanged, or if new media is added set to the default specified"
                            ;
                    GetLibraryFunctionSignature("llGetLinkMedia").DocumentationString =
                        "Get the media params for a particular face on linked prim, given the desired list of names. Returns a list of values in the order requested.  Returns an empty list if no media exists on the face"
                            ;
                    GetLibraryFunctionSignature("llClearLinkMedia").DocumentationString =
                        "Clears (deletes) the media and all params from the given face on linked prim";
                    GetLibraryFunctionSignature("llSetLinkCamera").DocumentationString =
                        "Sets the camera eye offset, and the offset that camera is looking at, for avatars that sit on the linked prim.;The two vector parameters are offsets relative to the object&apos;s center and expressed in local coordinates"
                            ;
                    GetLibraryFunctionSignature("llSetContentType").DocumentationString =
                        "Set the Internet media type of an LSL HTTP server response.;content_type may be one of CONTENT_TYPE_TEXT (default) &quot;text/plain&quot;, or CONTENT_TYPE_HTML &quot;text/html&quot;, only valid for embedded browsers on content owned by the person viewing. Falls back to &quot;text/plain&quot; otherwise"
                            ;
                    GetLibraryFunctionSignature("llLinkSitTarget").DocumentationString =
                        "Set the sit location for this object (if offset == &lt;0,0,0&gt; clear it)";
                    GetLibraryFunctionSignature("llAvatarOnLinkSitTarget").DocumentationString =
                        "If an avatar is sitting on the sit target, return the avatar&apos;s key, NULL_KEY otherwise"
                            ;
                    GetLibraryFunctionSignature("llSetVelocity").DocumentationString =
                        "Sets an objects velocity, in local coords if local == TRUE (if the script is physical)"
                            ;
                    GetLibraryFunctionSignature("llSetAngularVelocity").DocumentationString =
                        "Sets an objects angular velocity, in local coords if local == TRUE (if the script is physical)"
                            ;
                    GetLibraryFunctionSignature("llSetPhysicsMaterial").DocumentationString =
                        "Sets the requested attributes of the root object&apos;s physics material";
                    GetLibraryFunctionSignature("llGetPhysicsMaterial").DocumentationString =
                        "Returns the gravity multiplier, restitution, friction, and density of the linkset as a list in that order"
                            ;
                    GetLibraryFunctionSignature("llGetMassMKS").DocumentationString =
                        "Returns the mass of the linkset in kilograms";
                    GetLibraryFunctionSignature("llManageEstateAccess").DocumentationString =
                        "To add or remove agents from the estate&apos;s agent access or ban lists or groups from the estate&apos;s group access list.;Only works for objects owned by the Estate Owner or an Estate Manager.;Returns TRUE if successful and FALSE if throttled, on invalid action, on invalid or null id, or if object owner is not allowed to manage the estate.;action can be any of; ESTATE_ACCESS_ALLOWED_[AGENT/GROUP]_[ADD/REMOVE] or ESTATE_ACCESS_BANNED_AGENT_[ADD/REMOVE]"
                            ;
                    GetLibraryFunctionSignature("llSetKeyframedMotion").DocumentationString =
                        "Requests that a nonphysical object be keyframed according to keyframe list";
                    GetLibraryFunctionSignature("llTransferLindenDollars").DocumentationString =
                        "Transfer amount of linden dollars (L$) from script owner to destination. Returns a key to a corresponding transaction_result event for the success of the transfer"
                            ;
                    GetLibraryFunctionSignature("llGetParcelMusicURL").DocumentationString =
                        "Returnss the streaming audio URL for the parcel of land on which the object is located"
                            ;
                    GetLibraryFunctionSignature("llSetRegionPos").DocumentationString =
                        "Sets the position anywhere within the region (if the object isn&apos;t physical)";
                    GetLibraryFunctionSignature("llGetAgentList").DocumentationString =
                        "Requests a list of agents currently in the region, limited by the scope parameter";
                    GetLibraryFunctionSignature("llAttachToAvatarTemp").DocumentationString =
                        "Attaches the object to the avatar who has granted permission to the script, with the exception that the object will not create new inventory for the user, and will disappear on detach or disconnect"
                            ;
                    GetLibraryFunctionSignature("llTeleportAgent").DocumentationString =
                        "Requests a teleport of avatar to a landmark stored in the object&apos;s inventory. If no landmark is provided (an empty string), the avatar is teleported to the location position in the current region. In either case, the avatar is turned to face the position given by look_at in local coordinates"
                            ;
                    GetLibraryFunctionSignature("llTeleportAgentGlobalCoords").DocumentationString =
                        "Teleports an agent to set of a region_coordinates within a region at the specified global_coordinates. The agent lands facing the position defined by look_at local coordinates.;A region&apos;s global coordinates can be retrieved using llRequestSimulatorData(region_name, DATA_SIM_POS)"
                            ;
                    GetLibraryFunctionSignature("llGenerateKey").DocumentationString =
                        "Return a unique generated key";
                    GetLibraryFunctionSignature("llNavigateTo").DocumentationString =
                        "For AI Character: Navigate to destination";
                    GetLibraryFunctionSignature("llCreateCharacter").DocumentationString =
                        "Convert linkset to AI Character which can navigate the world";
                    GetLibraryFunctionSignature("llPursue").DocumentationString =
                        "For AI Character: Chase after a target";
                    GetLibraryFunctionSignature("llWanderWithin").DocumentationString =
                        "For AI Character: Wander within a specified volume";
                    GetLibraryFunctionSignature("llFleeFrom").DocumentationString =
                        "For AI Character: Flee from a point";
                    GetLibraryFunctionSignature("llPatrolPoints").DocumentationString =
                        "For AI Character: Patrol a list of points";
                    GetLibraryFunctionSignature("llExecCharacterCmd").DocumentationString =
                        "For AI Character: Execute a character command";
                    GetLibraryFunctionSignature("llDeleteCharacter").DocumentationString =
                        "Convert linkset from AI Character to Physics object";
                    GetLibraryFunctionSignature("llUpdateCharacter").DocumentationString =
                        "Change the AI Character&apos;s settings";
                    GetLibraryFunctionSignature("llEvade").DocumentationString =
                        "For AI Character: Evade a specified target";
                    GetLibraryFunctionSignature("llGetClosestNavPoint").DocumentationString =
                        "For AI Character: Get the closest navigable point to the point provided";
                    GetLibraryFunctionSignature("llGetStaticPath").DocumentationString =
                        "Returns a list of position vectors indicating pathfinding waypoints between positions at start and end, for a character of a given radius"
                            ;
                    GetLibraryFunctionSignature("llGetSimStats").DocumentationString =
                        "Returns the value of a particular simulator statistic";
                    GetLibraryFunctionSignature("llSetAnimationOverride").DocumentationString =
                        "Set the animation (anim) that will play for the given animation state (anim_state)".UnescapeXml
                            ();
                    GetLibraryFunctionSignature("llGetAnimationOverride").DocumentationString =
                        "Returns a string that is the name of the animation that is being used for the specified animation state (anim_state)"
                            ;
                    GetLibraryFunctionSignature("llResetAnimationOverride").DocumentationString =
                        "Resets the animation override of the specified animation state (anim_state) to the corresponding default value"
                            ;
                    GetLibraryFunctionSignature("llJson2List").DocumentationString =
                        "Converts the top level of the json string to a list";
                    GetLibraryFunctionSignature("llList2Json").DocumentationString =
                        "Converts either a strided list of key;value pairs to a JSON_OBJECT or a list of values to a JSON_ARRAY"
                            ;
                    GetLibraryFunctionSignature("llJsonGetValue").DocumentationString =
                        "Returns the value indicated by specifiers from the json string";
                    GetLibraryFunctionSignature("llJsonSetValue").DocumentationString =
                        "Returns a new json string that is the json given with the value indicated by specifiers set to value"
                            ;
                    GetLibraryFunctionSignature("llJsonValueType").DocumentationString =
                        "Returns the type constant for the value in json indicated by specifiers";
                    GetLibraryFunctionSignature("llReturnObjectsByID").DocumentationString =
                        "Returns a list of objects by their IDs";
                    GetLibraryFunctionSignature("llReturnObjectsByOwner").DocumentationString =
                        "Returns all objects of a particular owner in the given scope";
                    GetLibraryFunctionSignature("llScaleByFactor").DocumentationString =
                        "Uniformly resizes the linkset by the given multiplicative scale factor;Returns TRUE if rescaling was successful or FALSE otherwise"
                            ;
                    GetLibraryFunctionSignature("llGetMinScaleFactor").DocumentationString =
                        "Returns the minimum multiplicative scale factor which can be used by llScaleByFactor()"
                            ;
                    GetLibraryFunctionSignature("llGetMaxScaleFactor").DocumentationString =
                        "Returns the maximum multiplicative scale factor which can be used by llScaleByFactor()"
                            ;*/


                #endregion
            }
        }
    }

    [Flags]
    public enum LSLLibraryDataAdditions
    {
        None=0,
        OpenSimOssl =1,
        OpenSimWindlight=2,
        OpenSimBulletPhysics=4,
        OpenSimModInvoke=8
    }

    public enum LSLLibraryBaseData
    {
        StandardLsl,
        OpensimLsl,
        All
    }
}