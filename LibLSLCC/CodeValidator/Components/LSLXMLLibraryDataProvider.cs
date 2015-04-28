using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     A library data provider that reads LSL library data from XML, if you derive from this
    ///     class, the new class must have the LSLXmlLibraryDataRoot attribute
    /// </summary>
    [LSLXmlLibraryDataRoot]
    public class LSLXmlLibraryDataProvider : LSLLibraryDataProvider,
        IXmlSerializable
    {
        public LSLXmlLibraryDataProvider()
        {
        }



        private HashSet<string> _subsets = new HashSet<string>();



        /// <summary>
        ///     Builds a library data provider from an XML reader object
        /// </summary>
        /// <param name="data">The xml reader to read from</param>
        /// <param name="subsets">
        /// Data nodes must contain one of these subset strings in their Subsets property, otherwise they are discarded. 
        /// when "all" is used, all nodes are added and duplicates are accumulated into DuplicateEventsDefined, DuplicateConstantsDefined
        /// and DuplicateFunctionsDefined</param>
        /// <exception cref="ArgumentNullException">When data is null</exception>
        /// <exception cref="XmlException">When a syntax error is encountered</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public LSLXmlLibraryDataProvider(XmlTextReader data, IReadOnlySet<string> subsets)
        {
            FillFromXml(data, subsets);




            #region eventHandlers


            /*
            AddValidEventHandler("at_rot_target",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "handle"), new LSLParameter(LSLType.Rotation, "targetrot"),
                    new LSLParameter(LSLType.Rotation, "ourrot")
                });

            AddValidEventHandler("at_target",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "tnum"), new LSLParameter(LSLType.Vector, "targetpos"),
                    new LSLParameter(LSLType.Vector, "ourpos")
                });

            AddValidEventHandler("attach",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidEventHandler("changed",
                new[] { new LSLParameter(LSLType.Integer, "change") });

            AddValidEventHandler("collision",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("collision_end",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("collision_start",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("control",
                new[]
                {
                    new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.Integer, "level"),
                    new LSLParameter(LSLType.Integer, "edge")
                });

            AddValidEventHandler("dataserver",
                new[] { new LSLParameter(LSLType.Key, "queryid"), new LSLParameter(LSLType.String, "data") });

            AddValidEventHandler("email",
                new[]
                {
                    new LSLParameter(LSLType.String, "time"), new LSLParameter(LSLType.String, "address"),
                    new LSLParameter(LSLType.String, "subject"), new LSLParameter(LSLType.String, "message"),
                    new LSLParameter(LSLType.Integer, "num_left")
                });

            AddValidEventHandler("http_request",
                new[]
                {
                    new LSLParameter(LSLType.Key, "request_id"), new LSLParameter(LSLType.String, "method"),
                    new LSLParameter(LSLType.String, "body")
                });

            AddValidEventHandler("http_response",
                new[]
                {
                    new LSLParameter(LSLType.Key, "request_id"), new LSLParameter(LSLType.Integer, "status"),
                    new LSLParameter(LSLType.List, "metadata"), new LSLParameter(LSLType.String, "body")
                });

            AddValidEventHandler("land_collision",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidEventHandler("land_collision_end",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidEventHandler("land_collision_start",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidEventHandler("link_message",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "sender_num"), new LSLParameter(LSLType.Integer, "num"),
                    new LSLParameter(LSLType.String, "str"), new LSLParameter(LSLType.Key, "id")
                });

            AddValidEventHandler("listen",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "channel"), new LSLParameter(LSLType.String, "name"),
                    new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.String, "message")
                });

            AddValidEventHandler("money",
                new[] { new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.Integer, "amount") });

            AddValidEventHandler("moving_end", null);

            AddValidEventHandler("moving_start", null);

            AddValidEventHandler("no_sensor", null);

            AddValidEventHandler("not_at_rot_target", null);

            AddValidEventHandler("not_at_target", null);

            AddValidEventHandler("object_rez",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidEventHandler("on_rez",
                new[] { new LSLParameter(LSLType.Integer, "start_param") });

            AddValidEventHandler("path_update",
                new[] { new LSLParameter(LSLType.Integer, "type"), new LSLParameter(LSLType.List, "reserved") });

            AddValidEventHandler("remote_data",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "event_type"), new LSLParameter(LSLType.Key, "channel"),
                    new LSLParameter(LSLType.Key, "message_id"), new LSLParameter(LSLType.String, "sender"),
                    new LSLParameter(LSLType.Integer, "idata"), new LSLParameter(LSLType.String, "sdata")
                });

            AddValidEventHandler("run_time_permissions",
                new[] { new LSLParameter(LSLType.Integer, "perm") });

            AddValidEventHandler("sensor",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("state_entry", null);

            AddValidEventHandler("state_exit", null);

            AddValidEventHandler("timer", null);

            AddValidEventHandler("touch",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("touch_end",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("touch_start",
                new[] { new LSLParameter(LSLType.Integer, "num_detected") });

            AddValidEventHandler("transaction_result",
                new[]
                {
                    new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.Integer, "success"),
                    new LSLParameter(LSLType.String, "data")
                });

            */


            #endregion




            #region libraryFunctions


            /*
            AddValidLibraryFunction(LSLType.Integer, "llAbs",
                new[] { new LSLParameter(LSLType.Integer, "val") });

            AddValidLibraryFunction(LSLType.Float, "llAcos",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Void, "llAddToLandBanList",
                new[] { new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.Float, "hours") });

            AddValidLibraryFunction(LSLType.Void, "llAddToLandPassList",
                new[] { new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.Float, "hours") });

            AddValidLibraryFunction(LSLType.Void, "llAdjustSoundVolume",
                new[] { new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.Void, "llAllowInventoryDrop",
                new[] { new LSLParameter(LSLType.Integer, "add") });

            AddValidLibraryFunction(LSLType.Float, "llAngleBetween",
                new[] { new LSLParameter(LSLType.Rotation, "a"), new LSLParameter(LSLType.Rotation, "b") });

            AddValidLibraryFunction(LSLType.Void, "llApplyImpulse",
                new[] { new LSLParameter(LSLType.Vector, "momentum"), new LSLParameter(LSLType.Integer, "local") });

            AddValidLibraryFunction(LSLType.Void, "llApplyRotationalImpulse",
                new[] { new LSLParameter(LSLType.Vector, "force"), new LSLParameter(LSLType.Integer, "local") });

            AddValidLibraryFunction(LSLType.Float, "llAsin",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Float, "llAtan2",
                new[] { new LSLParameter(LSLType.Float, "y"), new LSLParameter(LSLType.Float, "x") });

            AddValidLibraryFunction(LSLType.Void, "llAttachToAvatar",
                new[] { new LSLParameter(LSLType.Integer, "attach_point") });

            AddValidLibraryFunction(LSLType.Void, "llAttachToAvatarTemp",
                new[] { new LSLParameter(LSLType.Integer, "attach_point") });

            AddValidLibraryFunction(LSLType.Key, "llAvatarOnLinkSitTarget",
                new[] { new LSLParameter(LSLType.Integer, "link") });

            AddValidLibraryFunction(LSLType.Key, "llAvatarOnSitTarget", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Rotation, "llAxes2Rot",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "fwd"), new LSLParameter(LSLType.Vector, "left"),
                    new LSLParameter(LSLType.Vector, "up")
                });

            AddValidLibraryFunction(LSLType.Rotation, "llAxisAngle2Rot",
                new[] { new LSLParameter(LSLType.Vector, "axis"), new LSLParameter(LSLType.Float, "angle") });

            AddValidLibraryFunction(LSLType.Integer, "llBase64ToInteger",
                new[] { new LSLParameter(LSLType.String, "str") });

            AddValidLibraryFunction(LSLType.String, "llBase64ToString",
                new[] { new LSLParameter(LSLType.String, "str") });

            AddValidLibraryFunction(LSLType.Void, "llBreakAllLinks", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llBreakLink",
                new[] { new LSLParameter(LSLType.Integer, "link") });

            AddValidLibraryFunction(LSLType.List, "llCSV2List",
                new[] { new LSLParameter(LSLType.String, "src") });

            AddValidLibraryFunction(LSLType.List, "llCastRay",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "start"), new LSLParameter(LSLType.Vector, "end"),
                    new LSLParameter(LSLType.List, "options")
                });

            AddValidLibraryFunction(LSLType.Integer, "llCeil",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Void, "llClearCameraParams", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llClearLinkMedia",
                new[] { new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Integer, "llClearPrimMedia",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Void, "llCloseRemoteDataChannel",
                new[] { new LSLParameter(LSLType.Key, "channel") });

            AddValidLibraryFunction(LSLType.Float, "llCloud",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Void, "llCollisionFilter",
                new[]
                {
                    new LSLParameter(LSLType.String, "name"), new LSLParameter(LSLType.Key, "id"),
                    new LSLParameter(LSLType.Integer, "accept")
                });

            AddValidLibraryFunction(LSLType.Void, "llCollisionSound",
                new[]
                {
                    new LSLParameter(LSLType.String, "impact_sound"),
                    new LSLParameter(LSLType.Float, "impact_volume")
                });

            AddValidLibraryFunction(LSLType.Void, "llCollisionSprite",
                new[] { new LSLParameter(LSLType.String, "impact_sprite") });

            AddValidLibraryFunction(LSLType.Float, "llCos",
                new[] { new LSLParameter(LSLType.Float, "theta") });

            AddValidLibraryFunction(LSLType.Void, "llCreateCharacter",
                new[] { new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Void, "llCreateLink",
                new[] { new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.Integer, "parent") });

            AddValidLibraryFunction(LSLType.Void, "llDeleteCharacter", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llDeleteSubList",
                new[]
                {
                    new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "start"),
                    new LSLParameter(LSLType.Integer, "end")
                });

            AddValidLibraryFunction(LSLType.String, "llDeleteSubString",
                new[]
                {
                    new LSLParameter(LSLType.String, "src"), new LSLParameter(LSLType.Integer, "start"),
                    new LSLParameter(LSLType.Integer, "end")
                });

            AddValidLibraryFunction(LSLType.Void, "llDetachFromAvatar", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedGrab",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Integer, "llDetectedGroup",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Key, "llDetectedKey",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Integer, "llDetectedLinkNumber",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.String, "llDetectedName",
                new[] { new LSLParameter(LSLType.Integer, "item") });

            AddValidLibraryFunction(LSLType.Key, "llDetectedOwner",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedPos",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Rotation, "llDetectedRot",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedTouchBinormal",
                new[] { new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Integer, "llDetectedTouchFace",
                new[] { new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedTouchNormal",
                new[] { new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedTouchPos",
                new[] { new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedTouchST",
                new[] { new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedTouchUV",
                new[] { new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Integer, "llDetectedType",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Vector, "llDetectedVel",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Void, "llDialog",
                new[]
                {
                    new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.String, "message"),
                    new LSLParameter(LSLType.List, "buttons"), new LSLParameter(LSLType.Integer, "channel")
                });

            AddValidLibraryFunction(LSLType.Void, "llDie", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llDumpList2String",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.String, "separator") });

            AddValidLibraryFunction(LSLType.Integer, "llEdgeOfWorld",
                new[] { new LSLParameter(LSLType.Vector, "pos"), new LSLParameter(LSLType.Vector, "dir") });

            AddValidLibraryFunction(LSLType.Void, "llEjectFromLand",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llEmail",
                new[]
                {
                    new LSLParameter(LSLType.String, "address"), new LSLParameter(LSLType.String, "subject"),
                    new LSLParameter(LSLType.String, "message")
                });

            AddValidLibraryFunction(LSLType.String, "llEscapeURL",
                new[] { new LSLParameter(LSLType.String, "url") });

            AddValidLibraryFunction(LSLType.Rotation, "llEuler2Rot",
                new[] { new LSLParameter(LSLType.Vector, "v") });

            AddValidLibraryFunction(LSLType.Void, "llEvade",
                new[] { new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Void, "llExecCharacterCmd",
                new[] { new LSLParameter(LSLType.Integer, "command"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Float, "llFabs",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Void, "llFleeFrom",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "position"), new LSLParameter(LSLType.Float, "distance"),
                    new LSLParameter(LSLType.List, "options")
                });

            AddValidLibraryFunction(LSLType.Integer, "llFloor",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Void, "llForceMouselook",
                new[] { new LSLParameter(LSLType.Integer, "mouselook") });

            AddValidLibraryFunction(LSLType.Float, "llFrand",
                new[] { new LSLParameter(LSLType.Float, "mag") });

            AddValidLibraryFunction(LSLType.Key, "llGenerateKey", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetAccel", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetAgentInfo",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.String, "llGetAgentLanguage",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.List, "llGetAgentList",
                new[] { new LSLParameter(LSLType.Integer, "scope"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Vector, "llGetAgentSize",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Float, "llGetAlpha",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Float, "llGetAndResetTime", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetAnimation",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.List, "llGetAnimationList",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.String, "llGetAnimationOverride",
                new[] { new LSLParameter(LSLType.String, "anim_state") });

            AddValidLibraryFunction(LSLType.Integer, "llGetAttached", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llGetBoundingBox",
                new[] { new LSLParameter(LSLType.Key, "object") });

            AddValidLibraryFunction(LSLType.Vector, "llGetCameraPos", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Rotation, "llGetCameraRot", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetCenterOfMass", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llGetClosestNavPoint",
                new[] { new LSLParameter(LSLType.Vector, "point"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Vector, "llGetColor",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Key, "llGetCreator", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetDate", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetDisplayName",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.Float, "llGetEnergy", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetEnv",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.Vector, "llGetForce", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetFreeMemory", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetFreeURLs", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetGMTclock", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetGeometricCenter", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetHTTPHeader",
                new[] { new LSLParameter(LSLType.Key, "request_id"), new LSLParameter(LSLType.String, "header") });

            AddValidLibraryFunction(LSLType.Key, "llGetInventoryCreator",
                new[] { new LSLParameter(LSLType.String, "item") });

            AddValidLibraryFunction(LSLType.Key, "llGetInventoryKey",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.String, "llGetInventoryName",
                new[] { new LSLParameter(LSLType.Integer, "type"), new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.Integer, "llGetInventoryNumber",
                new[] { new LSLParameter(LSLType.Integer, "type") });

            AddValidLibraryFunction(LSLType.Integer, "llGetInventoryPermMask",
                new[] { new LSLParameter(LSLType.String, "item"), new LSLParameter(LSLType.Integer, "category") });

            AddValidLibraryFunction(LSLType.Integer, "llGetInventoryType",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.Key, "llGetKey", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Key, "llGetLandOwnerAt",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Key, "llGetLinkKey",
                new[] { new LSLParameter(LSLType.Integer, "link") });

            AddValidLibraryFunction(LSLType.List, "llGetLinkMedia",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Integer, "face"),
                    new LSLParameter(LSLType.List, "params")
                });

            AddValidLibraryFunction(LSLType.String, "llGetLinkName",
                new[] { new LSLParameter(LSLType.Integer, "link") });

            AddValidLibraryFunction(LSLType.Integer, "llGetLinkNumber", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetLinkNumberOfSides",
                new[] { new LSLParameter(LSLType.Integer, "link") });

            AddValidLibraryFunction(LSLType.List, "llGetPrimitiveParams",
                new[] { new LSLParameter(LSLType.List, "params") });

            AddValidLibraryFunction(LSLType.Integer, "llGetListEntryType",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Integer, "llGetListLength",
                new[] { new LSLParameter(LSLType.List, "src") });

            AddValidLibraryFunction(LSLType.Vector, "llGetLocalPos", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Rotation, "llGetLocalRot", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetMass", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetMassMKS", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetMaxScaleFactor", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetMemoryLimit", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetMinScaleFactor", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llGetNextEmail",
                new[] { new LSLParameter(LSLType.String, "address"), new LSLParameter(LSLType.String, "subject") });

            AddValidLibraryFunction(LSLType.Key, "llGetNotecardLine",
                new[] { new LSLParameter(LSLType.String, "name"), new LSLParameter(LSLType.Integer, "line") });

            AddValidLibraryFunction(LSLType.Key, "llGetNumberOfNotecardLines",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.Integer, "llGetNumberOfPrims", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetNumberOfSides", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetObjectDesc", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llGetObjectDetails",
                new[] { new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.List, "params") });

            AddValidLibraryFunction(LSLType.Float, "llGetObjectMass",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.String, "llGetObjectName", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetObjectPermMask",
                new[] { new LSLParameter(LSLType.Integer, "category") });

            AddValidLibraryFunction(LSLType.Integer, "llGetObjectPrimCount",
                new[] { new LSLParameter(LSLType.Key, "prim") });

            AddValidLibraryFunction(LSLType.Vector, "llGetOmega", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Key, "llGetOwner", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Key, "llGetOwnerKey",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.List, "llGetParcelDetails",
                new[] { new LSLParameter(LSLType.Vector, "pos"), new LSLParameter(LSLType.List, "params") });

            AddValidLibraryFunction(LSLType.Integer, "llGetParcelFlags",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Integer, "llGetParcelMaxPrims",
                new[] { new LSLParameter(LSLType.Vector, "pos"), new LSLParameter(LSLType.Integer, "sim_wide") });

            AddValidLibraryFunction(LSLType.String, "llGetParcelMusicURL", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetParcelPrimCount",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "pos"), new LSLParameter(LSLType.Integer, "category"),
                    new LSLParameter(LSLType.Integer, "sim_wide")
                });

            AddValidLibraryFunction(LSLType.List, "llGetParcelPrimOwners",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Integer, "llGetPermissions", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Key, "llGetPermissionsKey", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llGetPhysicsMaterial", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetPos", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llGetPrimMediaParams",
                new[] { new LSLParameter(LSLType.Integer, "face"), new LSLParameter(LSLType.List, "params") });

            AddValidLibraryFunction(LSLType.List, "llGetLinkPrimitiveParams",
                new[] { new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.List, "params") });

            AddValidLibraryFunction(LSLType.Integer, "llGetRegionAgentCount", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetRegionCorner", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetRegionFPS", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetRegionFlags", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetRegionName", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetRegionTimeDilation", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetRootPosition", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Rotation, "llGetRootRotation", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Rotation, "llGetRot", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetSPMaxMemory", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetScale", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetScriptName", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetScriptState",
                new[] { new LSLParameter(LSLType.String, "script") });

            AddValidLibraryFunction(LSLType.Float, "llGetSimStats",
                new[] { new LSLParameter(LSLType.Integer, "stat_type") });

            AddValidLibraryFunction(LSLType.String, "llGetSimulatorHostname", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetStartParameter", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.List, "llGetStaticPath",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "start"), new LSLParameter(LSLType.Vector, "end"),
                    new LSLParameter(LSLType.Float, "radius"), new LSLParameter(LSLType.List, "params")
                });

            AddValidLibraryFunction(LSLType.Integer, "llGetStatus",
                new[] { new LSLParameter(LSLType.Integer, "status") });

            AddValidLibraryFunction(LSLType.String, "llGetSubString",
                new[]
                {
                    new LSLParameter(LSLType.String, "src"), new LSLParameter(LSLType.Integer, "start"),
                    new LSLParameter(LSLType.Integer, "end")
                });

            AddValidLibraryFunction(LSLType.Vector, "llGetSunDirection", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetTexture",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Vector, "llGetTextureOffset",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Float, "llGetTextureRot",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Vector, "llGetTextureScale",
                new[] { new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Float, "llGetTime", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetTimeOfDay", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetTimestamp", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Vector, "llGetTorque", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetUnixTime", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llGetUsedMemory", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.String, "llGetUsername",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.Vector, "llGetVel", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Float, "llGetWallclock", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llGiveInventory",
                new[] { new LSLParameter(LSLType.Key, "destination"), new LSLParameter(LSLType.String, "inventory") });

            AddValidLibraryFunction(LSLType.Void, "llGiveInventoryList",
                new[]
                {
                    new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.String, "folder"),
                    new LSLParameter(LSLType.List, "inventory")
                });

            AddValidLibraryFunction(LSLType.Integer, "llGiveMoney",
                new[] { new LSLParameter(LSLType.Key, "destination"), new LSLParameter(LSLType.Integer, "amount") });

            AddValidLibraryFunction(LSLType.Void, "llGodLikeRezObject",
                new[] { new LSLParameter(LSLType.Key, "inventory"), new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Float, "llGround",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Vector, "llGroundContour",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Vector, "llGroundNormal",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Void, "llGroundRepel",
                new[]
                {
                    new LSLParameter(LSLType.Float, "height"), new LSLParameter(LSLType.Integer, "water"),
                    new LSLParameter(LSLType.Float, "tau")
                });

            AddValidLibraryFunction(LSLType.Vector, "llGroundSlope",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Key, "llHTTPRequest",
                new[]
                {
                    new LSLParameter(LSLType.String, "url"), new LSLParameter(LSLType.List, "parameters"),
                    new LSLParameter(LSLType.String, "body")
                });

            AddValidLibraryFunction(LSLType.Void, "llHTTPResponse",
                new[]
                {
                    new LSLParameter(LSLType.Key, "request_id"), new LSLParameter(LSLType.Integer, "status"),
                    new LSLParameter(LSLType.String, "body")
                });

            AddValidLibraryFunction(LSLType.String, "llInsertString",
                new[]
                {
                    new LSLParameter(LSLType.String, "dst"), new LSLParameter(LSLType.Integer, "pos"),
                    new LSLParameter(LSLType.String, "src")
                });

            AddValidLibraryFunction(LSLType.Void, "llInstantMessage",
                new[] { new LSLParameter(LSLType.Key, "user"), new LSLParameter(LSLType.String, "message") });

            AddValidLibraryFunction(LSLType.String, "llIntegerToBase64",
                new[] { new LSLParameter(LSLType.Integer, "number") });

            AddValidLibraryFunction(LSLType.List, "llJson2List",
                new[] { new LSLParameter(LSLType.String, "src") });

            AddValidLibraryFunction(LSLType.String, "llJsonGetValue",
                new[] { new LSLParameter(LSLType.String, "json"), new LSLParameter(LSLType.List, "specifiers") });

            AddValidLibraryFunction(LSLType.String, "llJsonSetValue",
                new[]
                {
                    new LSLParameter(LSLType.String, "json"), new LSLParameter(LSLType.List, "specifiers"),
                    new LSLParameter(LSLType.String, "value")
                });

            AddValidLibraryFunction(LSLType.String, "llJsonValueType",
                new[] { new LSLParameter(LSLType.String, "json"), new LSLParameter(LSLType.List, "specifiers") });

            AddValidLibraryFunction(LSLType.String, "llKey2Name",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.Void, "llLinkSitTarget",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Vector, "offset"),
                    new LSLParameter(LSLType.Rotation, "rot")
                });

            AddValidLibraryFunction(LSLType.String, "llList2CSV",
                new[] { new LSLParameter(LSLType.List, "src") });

            AddValidLibraryFunction(LSLType.Float, "llList2Float",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Integer, "llList2Integer",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.String, "llList2Json",
                new[] { new LSLParameter(LSLType.String, "type"), new LSLParameter(LSLType.List, "values") });

            AddValidLibraryFunction(LSLType.Key, "llList2Key",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.List, "llList2List",
                new[]
                {
                    new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "start"),
                    new LSLParameter(LSLType.Integer, "end")
                });

            AddValidLibraryFunction(LSLType.List, "llList2ListStrided",
                new[]
                {
                    new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "start"),
                    new LSLParameter(LSLType.Integer, "end"), new LSLParameter(LSLType.Integer, "stride")
                });

            AddValidLibraryFunction(LSLType.Rotation, "llList2Rot",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.String, "llList2String",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Vector, "llList2Vector",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "index") });

            AddValidLibraryFunction(LSLType.Integer, "llListFindList",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.List, "test") });

            AddValidLibraryFunction(LSLType.List, "llListInsertList",
                new[]
                {
                    new LSLParameter(LSLType.List, "dest"), new LSLParameter(LSLType.List, "src"),
                    new LSLParameter(LSLType.Integer, "start")
                });

            AddValidLibraryFunction(LSLType.List, "llListRandomize",
                new[] { new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "stride") });

            AddValidLibraryFunction(LSLType.List, "llListReplaceList",
                new[]
                {
                    new LSLParameter(LSLType.List, "dest"), new LSLParameter(LSLType.List, "src"),
                    new LSLParameter(LSLType.Integer, "start"), new LSLParameter(LSLType.Integer, "end")
                });

            AddValidLibraryFunction(LSLType.List, "llListSort",
                new[]
                {
                    new LSLParameter(LSLType.List, "src"), new LSLParameter(LSLType.Integer, "stride"),
                    new LSLParameter(LSLType.Integer, "ascending")
                });

            AddValidLibraryFunction(LSLType.Float, "llListStatistics",
                new[] { new LSLParameter(LSLType.Integer, "operation"), new LSLParameter(LSLType.List, "src") });

            AddValidLibraryFunction(LSLType.Integer, "llListen",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "channel"), new LSLParameter(LSLType.String, "name"),
                    new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.String, "msg")
                });

            AddValidLibraryFunction(LSLType.Void, "llListenControl",
                new[] { new LSLParameter(LSLType.Integer, "handle"), new LSLParameter(LSLType.Integer, "active") });

            AddValidLibraryFunction(LSLType.Void, "llListenRemove",
                new[] { new LSLParameter(LSLType.Integer, "handle") });

            AddValidLibraryFunction(LSLType.Void, "llLoadURL",
                new[]
                {
                    new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.String, "message"),
                    new LSLParameter(LSLType.String, "url")
                });

            AddValidLibraryFunction(LSLType.Float, "llLog",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Float, "llLog10",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Void, "llLookAt",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "target"), new LSLParameter(LSLType.Float, "strength"),
                    new LSLParameter(LSLType.Float, "damping")
                });

            AddValidLibraryFunction(LSLType.Void, "llLoopSound",
                new[] { new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.Void, "llLoopSoundMaster",
                new[] { new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.Void, "llLoopSoundSlave",
                new[] { new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.String, "llMD5String",
                new[] { new LSLParameter(LSLType.String, "src"), new LSLParameter(LSLType.Integer, "nonce") });

            AddValidLibraryFunction(LSLType.Void, "llMakeExplosion",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "particles"), new LSLParameter(LSLType.Float, "scale"),
                    new LSLParameter(LSLType.Float, "vel"), new LSLParameter(LSLType.Float, "lifetime"),
                    new LSLParameter(LSLType.Float, "arc"), new LSLParameter(LSLType.String, "texture"),
                    new LSLParameter(LSLType.Vector, "offset")
                });

            AddValidLibraryFunction(LSLType.Void, "llMakeFire",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "particles"), new LSLParameter(LSLType.Float, "scale"),
                    new LSLParameter(LSLType.Float, "vel"), new LSLParameter(LSLType.Float, "lifetime"),
                    new LSLParameter(LSLType.Float, "arc"), new LSLParameter(LSLType.String, "texture"),
                    new LSLParameter(LSLType.Vector, "offset")
                });

            AddValidLibraryFunction(LSLType.Void, "llMakeFountain",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "particles"), new LSLParameter(LSLType.Float, "scale"),
                    new LSLParameter(LSLType.Float, "vel"), new LSLParameter(LSLType.Float, "lifetime"),
                    new LSLParameter(LSLType.Float, "arc"), new LSLParameter(LSLType.Integer, "bounce"),
                    new LSLParameter(LSLType.String, "texture"), new LSLParameter(LSLType.Vector, "offset"),
                    new LSLParameter(LSLType.Float, "bounce_offset")
                });

            AddValidLibraryFunction(LSLType.Void, "llMakeSmoke",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "particles"), new LSLParameter(LSLType.Float, "scale"),
                    new LSLParameter(LSLType.Float, "vel"), new LSLParameter(LSLType.Float, "lifetime"),
                    new LSLParameter(LSLType.Float, "arc"), new LSLParameter(LSLType.String, "texture"),
                    new LSLParameter(LSLType.Vector, "offset")
                });

            AddValidLibraryFunction(LSLType.Integer, "llManageEstateAccess",
                new[] { new LSLParameter(LSLType.Integer, "action"), new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llMapDestination",
                new[]
                {
                    new LSLParameter(LSLType.String, "simname"), new LSLParameter(LSLType.Vector, "pos"),
                    new LSLParameter(LSLType.Vector, "look_at")
                });

            AddValidLibraryFunction(LSLType.Integer, "llMatchGroup",
                new[] { new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.List, "group_keys") });

            AddValidLibraryFunction(LSLType.Void, "llMessageLinked",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Integer, "num"),
                    new LSLParameter(LSLType.String, "str"), new LSLParameter(LSLType.Key, "id")
                });

            AddValidLibraryFunction(LSLType.Void, "llMinEventDelay",
                new[] { new LSLParameter(LSLType.Float, "delay") });

            AddValidLibraryFunction(LSLType.Integer, "llModPow",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "a"), new LSLParameter(LSLType.Integer, "b"),
                    new LSLParameter(LSLType.Integer, "c")
                });

            AddValidLibraryFunction(LSLType.Void, "llModifyLand",
                new[] { new LSLParameter(LSLType.Integer, "action"), new LSLParameter(LSLType.Integer, "brush") });

            AddValidLibraryFunction(LSLType.Void, "llMoveToTarget",
                new[] { new LSLParameter(LSLType.Vector, "target"), new LSLParameter(LSLType.Float, "tau") });

            AddValidLibraryFunction(LSLType.Void, "llNavigateTo",
                new[] { new LSLParameter(LSLType.Vector, "pos"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Void, "llOffsetTexture",
                new[]
                {
                    new LSLParameter(LSLType.Float, "u"), new LSLParameter(LSLType.Float, "v"),
                    new LSLParameter(LSLType.Integer, "face")
                });

            AddValidLibraryFunction(LSLType.Void, "llOpenRemoteDataChannel", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llOverMyLand",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.Void, "llOwnerSay",
                new[] { new LSLParameter(LSLType.String, "msg") });

            AddValidLibraryFunction(LSLType.Void, "llParcelMediaCommandList",
                new[] { new LSLParameter(LSLType.List, "commandList") });

            AddValidLibraryFunction(LSLType.List, "llParcelMediaQuery",
                new[] { new LSLParameter(LSLType.List, "query") });

            AddValidLibraryFunction(LSLType.List, "llParseString2List",
                new[]
                {
                    new LSLParameter(LSLType.String, "src"), new LSLParameter(LSLType.List, "separators"),
                    new LSLParameter(LSLType.List, "spacers")
                });

            AddValidLibraryFunction(LSLType.List, "llParseStringKeepNulls",
                new[]
                {
                    new LSLParameter(LSLType.String, "src"), new LSLParameter(LSLType.List, "separators"),
                    new LSLParameter(LSLType.List, "spacers")
                });

            AddValidLibraryFunction(LSLType.Void, "llParticleSystem",
                new[] { new LSLParameter(LSLType.List, "rules") });

            AddValidLibraryFunction(LSLType.Void, "llLinkParticleSystem",
                new[] { new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.List, "rules") });

            AddValidLibraryFunction(LSLType.Void, "llPassCollisions",
                new[] { new LSLParameter(LSLType.Integer, "pass") });

            AddValidLibraryFunction(LSLType.Void, "llPassTouches",
                new[] { new LSLParameter(LSLType.Integer, "pass") });

            AddValidLibraryFunction(LSLType.Void, "llPatrolPoints",
                new[] { new LSLParameter(LSLType.List, "patrolPoints"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Void, "llPlaySound",
                new[] { new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.Void, "llPlaySoundSlave",
                new[] { new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.Void, "llPointAt",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Float, "llPow",
                new[] { new LSLParameter(LSLType.Float, "base"), new LSLParameter(LSLType.Float, "exponent") });

            AddValidLibraryFunction(LSLType.Void, "llPreloadSound",
                new[] { new LSLParameter(LSLType.String, "sound") });

            AddValidLibraryFunction(LSLType.Void, "llPursue",
                new[] { new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Void, "llPushObject",
                new[]
                {
                    new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.Vector, "impulse"),
                    new LSLParameter(LSLType.Vector, "ang_impulse"), new LSLParameter(LSLType.Integer, "local")
                });

            AddValidLibraryFunction(LSLType.Void, "llRefreshPrimURL", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llRegionSay",
                new[] { new LSLParameter(LSLType.Integer, "channel"), new LSLParameter(LSLType.String, "msg") });

            AddValidLibraryFunction(LSLType.Void, "llRegionSayTo",
                new[]
                {
                    new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.Integer, "channel"),
                    new LSLParameter(LSLType.String, "msg")
                });

            AddValidLibraryFunction(LSLType.Void, "llReleaseCamera",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llReleaseControls", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llReleaseURL",
                new[] { new LSLParameter(LSLType.String, "url") });

            AddValidLibraryFunction(LSLType.Void, "llRemoteDataReply",
                new[]
                {
                    new LSLParameter(LSLType.Key, "channel"), new LSLParameter(LSLType.Key, "message_id"),
                    new LSLParameter(LSLType.String, "sdata"), new LSLParameter(LSLType.Integer, "idata")
                });

            AddValidLibraryFunction(LSLType.Void, "llRemoteDataSetRegion", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llRemoteLoadScript",
                new[]
                {
                    new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.String, "name"),
                    new LSLParameter(LSLType.Integer, "running"),
                    new LSLParameter(LSLType.Integer, "start_param")
                });

            AddValidLibraryFunction(LSLType.Void, "llRemoteLoadScriptPin",
                new[]
                {
                    new LSLParameter(LSLType.Key, "target"), new LSLParameter(LSLType.String, "name"),
                    new LSLParameter(LSLType.Integer, "pin"), new LSLParameter(LSLType.Integer, "running"),
                    new LSLParameter(LSLType.Integer, "start_param")
                });

            AddValidLibraryFunction(LSLType.Void, "llRemoveFromLandBanList",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llRemoveFromLandPassList",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llRemoveInventory",
                new[] { new LSLParameter(LSLType.String, "item") });

            AddValidLibraryFunction(LSLType.Void, "llRemoveVehicleFlags",
                new[] { new LSLParameter(LSLType.Integer, "flags") });

            AddValidLibraryFunction(LSLType.Key, "llRequestAgentData",
                new[] { new LSLParameter(LSLType.Key, "id"), new LSLParameter(LSLType.Integer, "data") });

            AddValidLibraryFunction(LSLType.Key, "llRequestDisplayName",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.Key, "llRequestInventoryData",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.Void, "llRequestPermissions",
                new[] { new LSLParameter(LSLType.Key, "agent"), new LSLParameter(LSLType.Integer, "permissions") });

            AddValidLibraryFunction(LSLType.Key, "llRequestSecureURL", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Key, "llRequestSimulatorData",
                new[] { new LSLParameter(LSLType.String, "region"), new LSLParameter(LSLType.Integer, "data") });

            AddValidLibraryFunction(LSLType.Key, "llRequestURL", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Key, "llRequestUsername",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.Void, "llResetAnimationOverride",
                new[] { new LSLParameter(LSLType.String, "anim_state") });

            AddValidLibraryFunction(LSLType.Void, "llResetLandBanList", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llResetLandPassList", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llResetOtherScript",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.Void, "llResetScript", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llResetTime", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llReturnObjectsByID",
                new[] { new LSLParameter(LSLType.List, "objects") });

            AddValidLibraryFunction(LSLType.Integer, "llReturnObjectsByOwner",
                new[] { new LSLParameter(LSLType.Key, "owner"), new LSLParameter(LSLType.Integer, "scope") });

            AddValidLibraryFunction(LSLType.Void, "llRezAtRoot",
                new[]
                {
                    new LSLParameter(LSLType.String, "inventory"), new LSLParameter(LSLType.Vector, "position"),
                    new LSLParameter(LSLType.Vector, "velocity"), new LSLParameter(LSLType.Rotation, "rot"),
                    new LSLParameter(LSLType.Integer, "param")
                });

            AddValidLibraryFunction(LSLType.Void, "llRezObject",
                new[]
                {
                    new LSLParameter(LSLType.String, "inventory"), new LSLParameter(LSLType.Vector, "pos"),
                    new LSLParameter(LSLType.Vector, "vel"), new LSLParameter(LSLType.Rotation, "rot"),
                    new LSLParameter(LSLType.Integer, "param")
                });

            AddValidLibraryFunction(LSLType.Float, "llRot2Angle",
                new[] { new LSLParameter(LSLType.Rotation, "rot") });

            AddValidLibraryFunction(LSLType.Vector, "llRot2Axis",
                new[] { new LSLParameter(LSLType.Rotation, "rot") });

            AddValidLibraryFunction(LSLType.Vector, "llRot2Euler",
                new[] { new LSLParameter(LSLType.Rotation, "quat") });

            AddValidLibraryFunction(LSLType.Vector, "llRot2Fwd",
                new[] { new LSLParameter(LSLType.Rotation, "q") });

            AddValidLibraryFunction(LSLType.Vector, "llRot2Left",
                new[] { new LSLParameter(LSLType.Rotation, "q") });

            AddValidLibraryFunction(LSLType.Vector, "llRot2Up",
                new[] { new LSLParameter(LSLType.Rotation, "q") });

            AddValidLibraryFunction(LSLType.Rotation, "llRotBetween",
                new[] { new LSLParameter(LSLType.Vector, "start"), new LSLParameter(LSLType.Vector, "end") });

            AddValidLibraryFunction(LSLType.Void, "llRotLookAt",
                new[]
                {
                    new LSLParameter(LSLType.Rotation, "target_direction"),
                    new LSLParameter(LSLType.Float, "strength"), new LSLParameter(LSLType.Float, "damping")
                });

            AddValidLibraryFunction(LSLType.Integer, "llRotTarget",
                new[] { new LSLParameter(LSLType.Rotation, "rot"), new LSLParameter(LSLType.Float, "error") });

            AddValidLibraryFunction(LSLType.Void, "llRotTargetRemove",
                new[] { new LSLParameter(LSLType.Integer, "handle") });

            AddValidLibraryFunction(LSLType.Void, "llRotateTexture",
                new[] { new LSLParameter(LSLType.Float, "angle"), new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Integer, "llRound",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.String, "llSHA1String",
                new[] { new LSLParameter(LSLType.String, "src") });

            AddValidLibraryFunction(LSLType.Integer, "llSameGroup",
                new[] { new LSLParameter(LSLType.Key, "uuid") });

            AddValidLibraryFunction(LSLType.Void, "llSay",
                new[] { new LSLParameter(LSLType.Integer, "channel"), new LSLParameter(LSLType.String, "msg") });

            AddValidLibraryFunction(LSLType.Integer, "llScaleByFactor",
                new[] { new LSLParameter(LSLType.Float, "scaling_factor") });

            AddValidLibraryFunction(LSLType.Void, "llScaleTexture",
                new[]
                {
                    new LSLParameter(LSLType.Float, "u"), new LSLParameter(LSLType.Float, "v"),
                    new LSLParameter(LSLType.Integer, "face")
                });

            AddValidLibraryFunction(LSLType.Integer, "llScriptDanger",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Void, "llScriptProfiler",
                new[] { new LSLParameter(LSLType.Integer, "flags") });

            AddValidLibraryFunction(LSLType.Key, "llSendRemoteData",
                new[]
                {
                    new LSLParameter(LSLType.Key, "channel"), new LSLParameter(LSLType.String, "dest"),
                    new LSLParameter(LSLType.Integer, "idata"), new LSLParameter(LSLType.String, "sdata")
                });

            AddValidLibraryFunction(LSLType.Void, "llSensor",
                new[]
                {
                    new LSLParameter(LSLType.String, "name"), new LSLParameter(LSLType.Key, "id"),
                    new LSLParameter(LSLType.Integer, "type"), new LSLParameter(LSLType.Float, "range"),
                    new LSLParameter(LSLType.Float, "arc")
                });

            AddValidLibraryFunction(LSLType.Void, "llSensorRemove", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llSensorRepeat",
                new[]
                {
                    new LSLParameter(LSLType.String, "name"), new LSLParameter(LSLType.Key, "id"),
                    new LSLParameter(LSLType.Integer, "type"), new LSLParameter(LSLType.Float, "range"),
                    new LSLParameter(LSLType.Float, "arc"), new LSLParameter(LSLType.Float, "rate")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetAlpha",
                new[] { new LSLParameter(LSLType.Float, "alpha"), new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Void, "llSetAngularVelocity",
                new[] { new LSLParameter(LSLType.Vector, "initial_omega"), new LSLParameter(LSLType.Integer, "local") });

            AddValidLibraryFunction(LSLType.Void, "llSetAnimationOverride",
                new[] { new LSLParameter(LSLType.String, "anim_state"), new LSLParameter(LSLType.String, "anim") });

            AddValidLibraryFunction(LSLType.Void, "llSetBuoyancy",
                new[] { new LSLParameter(LSLType.Float, "buoyancy") });

            AddValidLibraryFunction(LSLType.Void, "llSetCameraAtOffset",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkCamera",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Vector, "eye"),
                    new LSLParameter(LSLType.Vector, "at")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetCameraEyeOffset",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Void, "llSetCameraParams",
                new[] { new LSLParameter(LSLType.List, "rules") });

            AddValidLibraryFunction(LSLType.Void, "llSetClickAction",
                new[] { new LSLParameter(LSLType.Integer, "action") });

            AddValidLibraryFunction(LSLType.Void, "llSetColor",
                new[] { new LSLParameter(LSLType.Vector, "color"), new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Void, "llSetContentType",
                new[] { new LSLParameter(LSLType.Key, "request_id"), new LSLParameter(LSLType.Integer, "content_type") });

            AddValidLibraryFunction(LSLType.Void, "llSetDamage",
                new[] { new LSLParameter(LSLType.Float, "damage") });

            AddValidLibraryFunction(LSLType.Void, "llSetForce",
                new[] { new LSLParameter(LSLType.Vector, "force"), new LSLParameter(LSLType.Integer, "local") });

            AddValidLibraryFunction(LSLType.Void, "llSetForceAndTorque",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "force"), new LSLParameter(LSLType.Vector, "torque"),
                    new LSLParameter(LSLType.Integer, "local")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetHoverHeight",
                new[]
                {
                    new LSLParameter(LSLType.Float, "height"), new LSLParameter(LSLType.Integer, "water"),
                    new LSLParameter(LSLType.Float, "tau")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetInventoryPermMask",
                new[]
                {
                    new LSLParameter(LSLType.String, "item"), new LSLParameter(LSLType.Integer, "category"),
                    new LSLParameter(LSLType.Integer, "value")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetKeyframedMotion",
                new[] { new LSLParameter(LSLType.List, "keyframes"), new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkAlpha",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Float, "alpha"),
                    new LSLParameter(LSLType.Integer, "face")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkColor",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Vector, "color"),
                    new LSLParameter(LSLType.Integer, "face")
                });

            AddValidLibraryFunction(LSLType.Integer, "llSetLinkMedia",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Integer, "face"),
                    new LSLParameter(LSLType.List, "params")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkTexture",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.String, "texture"),
                    new LSLParameter(LSLType.Integer, "face")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkTextureAnim",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.Integer, "mode"),
                    new LSLParameter(LSLType.Integer, "face"), new LSLParameter(LSLType.Integer, "sizex"),
                    new LSLParameter(LSLType.Integer, "sizey"), new LSLParameter(LSLType.Float, "start"),
                    new LSLParameter(LSLType.Float, "length"), new LSLParameter(LSLType.Float, "rate")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetLocalRot",
                new[] { new LSLParameter(LSLType.Rotation, "rot") });

            AddValidLibraryFunction(LSLType.Integer, "llSetMemoryLimit",
                new[] { new LSLParameter(LSLType.Integer, "limit") });

            AddValidLibraryFunction(LSLType.Void, "llSetObjectDesc",
                new[] { new LSLParameter(LSLType.String, "description") });

            AddValidLibraryFunction(LSLType.Void, "llSetObjectName",
                new[] { new LSLParameter(LSLType.String, "name") });

            AddValidLibraryFunction(LSLType.Void, "llSetObjectPermMask",
                new[] { new LSLParameter(LSLType.Integer, "mask"), new LSLParameter(LSLType.Integer, "value") });

            AddValidLibraryFunction(LSLType.Void, "llSetParcelMusicURL",
                new[] { new LSLParameter(LSLType.String, "url") });

            AddValidLibraryFunction(LSLType.Void, "llSetPayPrice",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "price"),
                    new LSLParameter(LSLType.List, "quick_pay_buttons")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetPhysicsMaterial",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "mask"),
                    new LSLParameter(LSLType.Float, "gravity_multiplier"),
                    new LSLParameter(LSLType.Float, "restitution"), new LSLParameter(LSLType.Float, "friction"),
                    new LSLParameter(LSLType.Float, "density")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetPos",
                new[] { new LSLParameter(LSLType.Vector, "pos") });

            AddValidLibraryFunction(LSLType.Integer, "llSetPrimMediaParams",
                new[] { new LSLParameter(LSLType.Integer, "face"), new LSLParameter(LSLType.List, "params") });

            AddValidLibraryFunction(LSLType.Void, "llSetPrimURL",
                new[] { new LSLParameter(LSLType.String, "url") });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkPrimitiveParamsFast",
                new[] { new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.List, "rules") });

            AddValidLibraryFunction(LSLType.Void, "llSetPrimitiveParams",
                new[] { new LSLParameter(LSLType.List, "rules") });

            AddValidLibraryFunction(LSLType.Void, "llSetLinkPrimitiveParams",
                new[] { new LSLParameter(LSLType.Integer, "link"), new LSLParameter(LSLType.List, "rules") });

            AddValidLibraryFunction(LSLType.Integer, "llSetRegionPos",
                new[] { new LSLParameter(LSLType.Vector, "position") });

            AddValidLibraryFunction(LSLType.Void, "llSetRemoteScriptAccessPin",
                new[] { new LSLParameter(LSLType.Integer, "pin") });

            AddValidLibraryFunction(LSLType.Void, "llSetRot",
                new[] { new LSLParameter(LSLType.Rotation, "rot") });

            AddValidLibraryFunction(LSLType.Void, "llSetScale",
                new[] { new LSLParameter(LSLType.Vector, "size") });

            AddValidLibraryFunction(LSLType.Void, "llSetScriptState",
                new[] { new LSLParameter(LSLType.String, "name"), new LSLParameter(LSLType.Integer, "running") });

            AddValidLibraryFunction(LSLType.Void, "llSetSculptAnim",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "mode"), new LSLParameter(LSLType.Integer, "sizex"),
                    new LSLParameter(LSLType.Integer, "sizey"), new LSLParameter(LSLType.Integer, "start_frame"),
                    new LSLParameter(LSLType.Integer, "end_frame"), new LSLParameter(LSLType.Float, "rate"),
                    new LSLParameter(LSLType.Integer, "texture_sync")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetSitText",
                new[] { new LSLParameter(LSLType.String, "text") });

            AddValidLibraryFunction(LSLType.Void, "llSetSoundQueueing",
                new[] { new LSLParameter(LSLType.Integer, "queue") });

            AddValidLibraryFunction(LSLType.Void, "llSetSoundRadius",
                new[] { new LSLParameter(LSLType.Float, "radius") });

            AddValidLibraryFunction(LSLType.Void, "llSetStatus",
                new[] { new LSLParameter(LSLType.Integer, "status"), new LSLParameter(LSLType.Integer, "value") });

            AddValidLibraryFunction(LSLType.Void, "llSetText",
                new[]
                {
                    new LSLParameter(LSLType.String, "text"), new LSLParameter(LSLType.Vector, "color"),
                    new LSLParameter(LSLType.Float, "alpha")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetTexture",
                new[] { new LSLParameter(LSLType.String, "texture"), new LSLParameter(LSLType.Integer, "face") });

            AddValidLibraryFunction(LSLType.Void, "llSetTextureAnim",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "mode"), new LSLParameter(LSLType.Integer, "face"),
                    new LSLParameter(LSLType.Integer, "sizex"), new LSLParameter(LSLType.Integer, "sizey"),
                    new LSLParameter(LSLType.Float, "start"), new LSLParameter(LSLType.Float, "length"),
                    new LSLParameter(LSLType.Float, "rate")
                });

            AddValidLibraryFunction(LSLType.Void, "llSetTimerEvent",
                new[] { new LSLParameter(LSLType.Float, "sec") });

            AddValidLibraryFunction(LSLType.Void, "llSetTorque",
                new[] { new LSLParameter(LSLType.Vector, "torque"), new LSLParameter(LSLType.Integer, "local") });

            AddValidLibraryFunction(LSLType.Void, "llSetTouchText",
                new[] { new LSLParameter(LSLType.String, "text") });

            AddValidLibraryFunction(LSLType.Void, "llSetVehicleFlags",
                new[] { new LSLParameter(LSLType.Integer, "flags") });

            AddValidLibraryFunction(LSLType.Void, "llSetVehicleFloatParam",
                new[] { new LSLParameter(LSLType.Integer, "param"), new LSLParameter(LSLType.Float, "value") });

            AddValidLibraryFunction(LSLType.Void, "llSetVehicleRotationParam",
                new[] { new LSLParameter(LSLType.Integer, "param"), new LSLParameter(LSLType.Rotation, "rot") });

            AddValidLibraryFunction(LSLType.Void, "llSetVehicleType",
                new[] { new LSLParameter(LSLType.Integer, "type") });

            AddValidLibraryFunction(LSLType.Void, "llSetVehicleVectorParam",
                new[] { new LSLParameter(LSLType.Integer, "param"), new LSLParameter(LSLType.Vector, "vec") });

            AddValidLibraryFunction(LSLType.Void, "llSetVelocity",
                new[] { new LSLParameter(LSLType.Vector, "force"), new LSLParameter(LSLType.Integer, "local") });

            AddValidLibraryFunction(LSLType.Void, "llShout",
                new[] { new LSLParameter(LSLType.Integer, "channel"), new LSLParameter(LSLType.String, "msg") });

            AddValidLibraryFunction(LSLType.Float, "llSin",
                new[] { new LSLParameter(LSLType.Float, "theta") });

            AddValidLibraryFunction(LSLType.Void, "llSitTarget",
                new[] { new LSLParameter(LSLType.Vector, "offset"), new LSLParameter(LSLType.Rotation, "rot") });

            AddValidLibraryFunction(LSLType.Void, "llSleep",
                new[] { new LSLParameter(LSLType.Float, "sec") });

            AddValidLibraryFunction(LSLType.Void, "llSound",
                new[]
                {
                    new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume"),
                    new LSLParameter(LSLType.Integer, "queue"), new LSLParameter(LSLType.Integer, "loop")
                });

            AddValidLibraryFunction(LSLType.Void, "llSoundPreload",
                new[] { new LSLParameter(LSLType.String, "sound") });

            AddValidLibraryFunction(LSLType.Float, "llSqrt",
                new[] { new LSLParameter(LSLType.Float, "val") });

            AddValidLibraryFunction(LSLType.Void, "llStartAnimation",
                new[] { new LSLParameter(LSLType.String, "anim") });

            AddValidLibraryFunction(LSLType.Void, "llStopAnimation",
                new[] { new LSLParameter(LSLType.String, "anim") });

            AddValidLibraryFunction(LSLType.Void, "llStopHover", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llStopLookAt", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llStopMoveToTarget", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llStopPointAt", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Void, "llStopSound", new LSLParameter[] { });

            AddValidLibraryFunction(LSLType.Integer, "llStringLength",
                new[] { new LSLParameter(LSLType.String, "str") });

            AddValidLibraryFunction(LSLType.String, "llStringToBase64",
                new[] { new LSLParameter(LSLType.String, "str") });

            AddValidLibraryFunction(LSLType.String, "llStringTrim",
                new[] { new LSLParameter(LSLType.String, "src"), new LSLParameter(LSLType.Integer, "type") });

            AddValidLibraryFunction(LSLType.Integer, "llSubStringIndex",
                new[] { new LSLParameter(LSLType.String, "source"), new LSLParameter(LSLType.String, "pattern") });

            AddValidLibraryFunction(LSLType.Void, "llTakeCamera",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llTakeControls",
                new[]
                {
                    new LSLParameter(LSLType.Integer, "controls"), new LSLParameter(LSLType.Integer, "accept"),
                    new LSLParameter(LSLType.Integer, "pass_on")
                });

            AddValidLibraryFunction(LSLType.Float, "llTan",
                new[] { new LSLParameter(LSLType.Float, "theta") });

            AddValidLibraryFunction(LSLType.Integer, "llTarget",
                new[] { new LSLParameter(LSLType.Vector, "position"), new LSLParameter(LSLType.Float, "range") });

            AddValidLibraryFunction(LSLType.Void, "llTargetOmega",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "axis"), new LSLParameter(LSLType.Float, "spinrate"),
                    new LSLParameter(LSLType.Float, "gain")
                });

            AddValidLibraryFunction(LSLType.Void, "llTargetRemove",
                new[] { new LSLParameter(LSLType.Integer, "handle") });

            AddValidLibraryFunction(LSLType.Void, "llTeleportAgent",
                new[]
                {
                    new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.String, "landmark"),
                    new LSLParameter(LSLType.Vector, "position"), new LSLParameter(LSLType.Vector, "look_at")
                });

            AddValidLibraryFunction(LSLType.Void, "llTeleportAgentGlobalCoords",
                new[]
                {
                    new LSLParameter(LSLType.Key, "agent"),
                    new LSLParameter(LSLType.Vector, "global_coordinates"),
                    new LSLParameter(LSLType.Vector, "region_coordinates"),
                    new LSLParameter(LSLType.Vector, "look_at")
                });

            AddValidLibraryFunction(LSLType.Void, "llTeleportAgentHome",
                new[] { new LSLParameter(LSLType.Key, "avatar") });

            AddValidLibraryFunction(LSLType.Void, "llTextBox",
                new[]
                {
                    new LSLParameter(LSLType.Key, "avatar"), new LSLParameter(LSLType.String, "message"),
                    new LSLParameter(LSLType.Integer, "channel")
                });

            AddValidLibraryFunction(LSLType.String, "llToLower",
                new[] { new LSLParameter(LSLType.String, "src") });

            AddValidLibraryFunction(LSLType.String, "llToUpper",
                new[] { new LSLParameter(LSLType.String, "src") });

            AddValidLibraryFunction(LSLType.Key, "llTransferLindenDollars",
                new[] { new LSLParameter(LSLType.Key, "destination"), new LSLParameter(LSLType.Integer, "amount") });

            AddValidLibraryFunction(LSLType.Void, "llTriggerSound",
                new[] { new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume") });

            AddValidLibraryFunction(LSLType.Void, "llTriggerSoundLimited",
                new[]
                {
                    new LSLParameter(LSLType.String, "sound"), new LSLParameter(LSLType.Float, "volume"),
                    new LSLParameter(LSLType.Vector, "top_north_east"),
                    new LSLParameter(LSLType.Vector, "bottom_south_west")
                });

            AddValidLibraryFunction(LSLType.Void, "llUnSit",
                new[] { new LSLParameter(LSLType.Key, "id") });

            AddValidLibraryFunction(LSLType.String, "llUnescapeURL",
                new[] { new LSLParameter(LSLType.String, "url") });

            AddValidLibraryFunction(LSLType.Void, "llUpdateCharacter",
                new[] { new LSLParameter(LSLType.List, "options") });

            AddValidLibraryFunction(LSLType.Float, "llVecDist",
                new[] { new LSLParameter(LSLType.Vector, "vec_a"), new LSLParameter(LSLType.Vector, "vec_b") });

            AddValidLibraryFunction(LSLType.Float, "llVecMag",
                new[] { new LSLParameter(LSLType.Vector, "vec") });

            AddValidLibraryFunction(LSLType.Vector, "llVecNorm",
                new[] { new LSLParameter(LSLType.Vector, "vec") });

            AddValidLibraryFunction(LSLType.Void, "llVolumeDetect",
                new[] { new LSLParameter(LSLType.Integer, "detect") });

            AddValidLibraryFunction(LSLType.Void, "llWanderWithin",
                new[]
                {
                    new LSLParameter(LSLType.Vector, "origin"), new LSLParameter(LSLType.Vector, "dist"),
                    new LSLParameter(LSLType.List, "options")
                });

            AddValidLibraryFunction(LSLType.Float, "llWater",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.Void, "llWhisper",
                new[] { new LSLParameter(LSLType.Integer, "channel"), new LSLParameter(LSLType.String, "msg") });

            AddValidLibraryFunction(LSLType.Vector, "llWind",
                new[] { new LSLParameter(LSLType.Vector, "offset") });

            AddValidLibraryFunction(LSLType.String, "llXorBase64",
                new[] { new LSLParameter(LSLType.String, "str1"), new LSLParameter(LSLType.String, "str2") });

            AddValidLibraryFunction(LSLType.String, "llXorBase64StringsCorrect",
                new[] { new LSLParameter(LSLType.String, "str1"), new LSLParameter(LSLType.String, "str2") });

            AddValidLibraryFunction(LSLType.String, "llXorBase64Strings",
                new[] { new LSLParameter(LSLType.String, "str1"), new LSLParameter(LSLType.String, "str2") });
            */


            #endregion




            #region constants


            /*
            AddValidConstant(LSLType.Integer, "PSYS_PART_START_ALPHA");

            AddValidConstant(LSLType.Integer, "PSYS_PART_END_ALPHA");

            AddValidConstant(LSLType.Integer, "PSYS_PART_START_COLOR");

            AddValidConstant(LSLType.Integer, "PSYS_PART_END_COLOR");

            AddValidConstant(LSLType.Integer, "PSYS_PART_START_SCALE");

            AddValidConstant(LSLType.Integer, "PSYS_PART_END_SCALE");

            AddValidConstant(LSLType.Integer, "PSYS_PART_MAX_AGE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_MAX_AGE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_ACCEL");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_ANGLE_BEGIN");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_ANGLE_END");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_BURST_PART_COUNT");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_BURST_RADIUS");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_BURST_RATE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_BURST_SPEED_MIN");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_BURST_SPEED_MAX");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_INNERANGLE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_OUTERANGLE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_OMEGA");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_TARGET_KEY");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_TEXTURE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_PATTERN");

            AddValidConstant(LSLType.Integer, "PSYS_PART_FLAGS");

            AddValidConstant(LSLType.Integer, "PSYS_PART_BOUNCE_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_EMISSIVE_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_FOLLOW_SRC_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_FOLLOW_VELOCITY_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_INTERP_COLOR_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_INTERP_SCALE_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_TARGET_LINEAR_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_TARGET_POS_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_PART_WIND_MASK");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_PATTERN_ANGLE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_PATTERN_ANGLE_CONE");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_PATTERN_ANGLE_CONE_EMPTY");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_PATTERN_DROP");

            AddValidConstant(LSLType.Integer, "PSYS_SRC_PATTERN_EXPLODE");

            AddValidConstant(LSLType.Integer, "ACTIVE");

            AddValidConstant(LSLType.Integer, "AGENT");

            AddValidConstant(LSLType.Integer, "AGENT_ALWAYS_RUN");

            AddValidConstant(LSLType.Integer, "AGENT_ATTACHMENTS");

            AddValidConstant(LSLType.Integer, "AGENT_AUTOPILOT");

            AddValidConstant(LSLType.Integer, "AGENT_AWAY");

            AddValidConstant(LSLType.Integer, "AGENT_BUSY");

            AddValidConstant(LSLType.Integer, "AGENT_BY_LEGACY_NAME");

            AddValidConstant(LSLType.Integer, "AGENT_BY_USERNAME");

            AddValidConstant(LSLType.Integer, "AGENT_CROUCHING");

            AddValidConstant(LSLType.Integer, "AGENT_FLYING");

            AddValidConstant(LSLType.Integer, "AGENT_IN_AIR");

            AddValidConstant(LSLType.Integer, "AGENT_LIST_PARCEL");

            AddValidConstant(LSLType.Integer, "AGENT_LIST_PARCEL_OWNER");

            AddValidConstant(LSLType.Integer, "AGENT_LIST_REGION");

            AddValidConstant(LSLType.Integer, "AGENT_MOUSELOOK");

            AddValidConstant(LSLType.Integer, "AGENT_ON_OBJECT");

            AddValidConstant(LSLType.Integer, "AGENT_SCRIPTED");

            AddValidConstant(LSLType.Integer, "AGENT_SITTING");

            AddValidConstant(LSLType.Integer, "AGENT_TYPING");

            AddValidConstant(LSLType.Integer, "AGENT_WALKING");

            AddValidConstant(LSLType.Integer, "ALL_SIDES");

            AddValidConstant(LSLType.Integer, "ANIM_ON");

            AddValidConstant(LSLType.Integer, "ATTACH_AVATAR_CENTER");

            AddValidConstant(LSLType.Integer, "ATTACH_BACK");

            AddValidConstant(LSLType.Integer, "ATTACH_BELLY");

            AddValidConstant(LSLType.Integer, "ATTACH_CHEST");

            AddValidConstant(LSLType.Integer, "ATTACH_CHIN");

            AddValidConstant(LSLType.Integer, "ATTACH_HEAD");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_BOTTOM");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_BOTTOM_LEFT");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_BOTTOM_RIGHT");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_CENTER_1");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_CENTER_2");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_TOP_CENTER");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_TOP_LEFT");

            AddValidConstant(LSLType.Integer, "ATTACH_HUD_TOP_RIGHT");

            AddValidConstant(LSLType.Integer, "ATTACH_LEAR");

            AddValidConstant(LSLType.Integer, "ATTACH_LEFT_PEC");

            AddValidConstant(LSLType.Integer, "ATTACH_RPEC");

            AddValidConstant(LSLType.Integer, "ATTACH_LEYE");

            AddValidConstant(LSLType.Integer, "ATTACH_LFOOT");

            AddValidConstant(LSLType.Integer, "ATTACH_LHAND");

            AddValidConstant(LSLType.Integer, "ATTACH_LHIP");

            AddValidConstant(LSLType.Integer, "ATTACH_LLARM");

            AddValidConstant(LSLType.Integer, "ATTACH_LLLEG");

            AddValidConstant(LSLType.Integer, "ATTACH_LSHOULDER");

            AddValidConstant(LSLType.Integer, "ATTACH_LUARM");

            AddValidConstant(LSLType.Integer, "ATTACH_LULEG");

            AddValidConstant(LSLType.Integer, "ATTACH_MOUTH");

            AddValidConstant(LSLType.Integer, "ATTACH_NECK");

            AddValidConstant(LSLType.Integer, "ATTACH_NOSE");

            AddValidConstant(LSLType.Integer, "ATTACH_PELVIS");

            AddValidConstant(LSLType.Integer, "ATTACH_REAR");

            AddValidConstant(LSLType.Integer, "ATTACH_REYE");

            AddValidConstant(LSLType.Integer, "ATTACH_RFOOT");

            AddValidConstant(LSLType.Integer, "ATTACH_RHAND");

            AddValidConstant(LSLType.Integer, "ATTACH_RHIP");

            AddValidConstant(LSLType.Integer, "ATTACH_RIGHT_PEC");

            AddValidConstant(LSLType.Integer, "ATTACH_LPEC");

            AddValidConstant(LSLType.Integer, "ATTACH_RLARM");

            AddValidConstant(LSLType.Integer, "ATTACH_RLLEG");

            AddValidConstant(LSLType.Integer, "ATTACH_RSHOULDER");

            AddValidConstant(LSLType.Integer, "ATTACH_RUARM");

            AddValidConstant(LSLType.Integer, "ATTACH_RULEG");

            AddValidConstant(LSLType.Integer, "CAMERA_ACTIVE");

            AddValidConstant(LSLType.Integer, "CAMERA_BEHINDNESS_ANGLE");

            AddValidConstant(LSLType.Integer, "CAMERA_BEHINDNESS_LAG");

            AddValidConstant(LSLType.Integer, "CAMERA_DISTANCE");

            AddValidConstant(LSLType.Integer, "CAMERA_FOCUS");

            AddValidConstant(LSLType.Integer, "CAMERA_FOCUS_LAG");

            AddValidConstant(LSLType.Integer, "CAMERA_FOCUS_LOCKED");

            AddValidConstant(LSLType.Integer, "CAMERA_FOCUS_OFFSET");

            AddValidConstant(LSLType.Integer, "CAMERA_FOCUS_THRESHOLD");

            AddValidConstant(LSLType.Integer, "CAMERA_PITCH");

            AddValidConstant(LSLType.Integer, "CAMERA_POSITION");

            AddValidConstant(LSLType.Integer, "CAMERA_POSITION_LAG");

            AddValidConstant(LSLType.Integer, "CAMERA_POSITION_LOCKED");

            AddValidConstant(LSLType.Integer, "CAMERA_POSITION_THRESHOLD");

            AddValidConstant(LSLType.Integer, "CHANGED_ALLOWED_DROP");

            AddValidConstant(LSLType.Integer, "CHANGED_COLOR");

            AddValidConstant(LSLType.Integer, "CHANGED_INVENTORY");

            AddValidConstant(LSLType.Integer, "CHANGED_LINK");

            AddValidConstant(LSLType.Integer, "CHANGED_MEDIA");

            AddValidConstant(LSLType.Integer, "CHANGED_OWNER");

            AddValidConstant(LSLType.Integer, "CHANGED_REGION");

            AddValidConstant(LSLType.Integer, "CHANGED_REGION_START");

            AddValidConstant(LSLType.Integer, "CHANGED_SCALE");

            AddValidConstant(LSLType.Integer, "CHANGED_SHAPE");

            AddValidConstant(LSLType.Integer, "CHANGED_TELEPORT");

            AddValidConstant(LSLType.Integer, "CHANGED_TEXTURE");

            AddValidConstant(LSLType.Integer, "CHARACTER_ACCOUNT_FOR_SKIPPED_FRAMES");

            AddValidConstant(LSLType.Integer, "CHARACTER_AVOIDANCE_MODE");

            AddValidConstant(LSLType.Integer, "CHARACTER_DESIRED_SPEED");

            AddValidConstant(LSLType.Integer, "CHARACTER_DESIRED_TURN_SPEED");

            AddValidConstant(LSLType.Integer, "CHARACTER_LENGTH");

            AddValidConstant(LSLType.Integer, "CHARACTER_MAX_ACCEL");

            AddValidConstant(LSLType.Integer, "CHARACTER_MAX_DECEL");

            AddValidConstant(LSLType.Integer, "CHARACTER_MAX_SPEED");

            AddValidConstant(LSLType.Integer, "CHARACTER_MAX_TURN_RADIUS");

            AddValidConstant(LSLType.Integer, "CHARACTER_ORIENTATION");

            AddValidConstant(LSLType.Integer, "CHARACTER_RADIUS");

            AddValidConstant(LSLType.Integer, "CHARACTER_STAY_WITHIN_PARCEL");

            AddValidConstant(LSLType.Integer, "CHARACTER_TYPE");

            AddValidConstant(LSLType.Integer, "CHARACTER_TYPE_A");

            AddValidConstant(LSLType.Integer, "CHARACTER_TYPE_B");

            AddValidConstant(LSLType.Integer, "CHARACTER_TYPE_C");

            AddValidConstant(LSLType.Integer, "CHARACTER_TYPE_D");

            AddValidConstant(LSLType.Integer, "CHARACTER_TYPE_NONE");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_BUY");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_NONE");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_OPEN");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_OPEN_MEDIA");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_PAY");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_PLAY");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_SIT");

            AddValidConstant(LSLType.Integer, "CLICK_ACTION_TOUCH");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_ATOM");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_FORM");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_HTML");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_JSON");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_LLSD");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_RSS");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_TEXT");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_XHTML");

            AddValidConstant(LSLType.Integer, "CONTENT_TYPE_XML");

            AddValidConstant(LSLType.Integer, "CONTROL_BACK");

            AddValidConstant(LSLType.Integer, "CONTROL_DOWN");

            AddValidConstant(LSLType.Integer, "CONTROL_FWD");

            AddValidConstant(LSLType.Integer, "CONTROL_LBUTTON");

            AddValidConstant(LSLType.Integer, "CONTROL_LEFT");

            AddValidConstant(LSLType.Integer, "CONTROL_ML_LBUTTON");

            AddValidConstant(LSLType.Integer, "CONTROL_RIGHT");

            AddValidConstant(LSLType.Integer, "CONTROL_ROT_LEFT");

            AddValidConstant(LSLType.Integer, "CONTROL_ROT_RIGHT");

            AddValidConstant(LSLType.Integer, "CONTROL_UP");

            AddValidConstant(LSLType.Integer, "DATA_BORN");

            AddValidConstant(LSLType.Integer, "DATA_NAME");

            AddValidConstant(LSLType.Integer, "DATA_ONLINE");

            AddValidConstant(LSLType.Integer, "DATA_PAYINFO");

            AddValidConstant(LSLType.Integer, "DATA_RATING");

            AddValidConstant(LSLType.Integer, "DATA_SIM_POS");

            AddValidConstant(LSLType.Integer, "DATA_SIM_RATING");

            AddValidConstant(LSLType.Integer, "DATA_SIM_STATUS");

            AddValidConstant(LSLType.Integer, "DEBUG_CHANNEL");

            AddValidConstant(LSLType.Float, "DEG_TO_RAD");

            AddValidConstant(LSLType.String, "EOF");

            AddValidConstant(LSLType.Integer, "ERR_GENERIC");

            AddValidConstant(LSLType.Integer, "ERR_MALFORMED_PARAMS");

            AddValidConstant(LSLType.Integer, "ERR_PARCEL_PERMISSIONS");

            AddValidConstant(LSLType.Integer, "ERR_RUNTIME_PERMISSIONS");

            AddValidConstant(LSLType.Integer, "ERR_THROTTLED");

            AddValidConstant(LSLType.Integer, "ESTATE_ACCESS_ALLOWED_AGENT_ADD");

            AddValidConstant(LSLType.Integer, "ESTATE_ACCESS_ALLOWED_AGENT_REMOVE");

            AddValidConstant(LSLType.Integer, "ESTATE_ACCESS_ALLOWED_GROUP_ADD");

            AddValidConstant(LSLType.Integer, "ESTATE_ACCESS_ALLOWED_GROUP_REMOVE");

            AddValidConstant(LSLType.Integer, "ESTATE_ACCESS_BANNED_AGENT_ADD");

            AddValidConstant(LSLType.Integer, "ESTATE_ACCESS_BANNED_AGENT_REMOVE");

            AddValidConstant(LSLType.Integer, "FALSE");

            AddValidConstant(LSLType.Integer, "FORCE_DIRECT_PATH");

            AddValidConstant(LSLType.Integer, "HORIZONTAL");

            AddValidConstant(LSLType.Integer, "HTTP_BODY_MAXLENGTH");

            AddValidConstant(LSLType.Integer, "HTTP_BODY_TRUNCATED");

            AddValidConstant(LSLType.Integer, "HTTP_CUSTOM_HEADER");

            AddValidConstant(LSLType.Integer, "HTTP_METHOD");

            AddValidConstant(LSLType.Integer, "HTTP_MIMETYPE");

            AddValidConstant(LSLType.Integer, "HTTP_PRAGMA_NO_CACHE");

            AddValidConstant(LSLType.Integer, "HTTP_VERBOSE_THROTTLE");

            AddValidConstant(LSLType.Integer, "HTTP_VERIFY_CERT");

            AddValidConstant(LSLType.Integer, "INVENTORY_ALL");

            AddValidConstant(LSLType.Integer, "INVENTORY_ANIMATION");

            AddValidConstant(LSLType.Integer, "INVENTORY_BODYPART");

            AddValidConstant(LSLType.Integer, "INVENTORY_CLOTHING");

            AddValidConstant(LSLType.Integer, "INVENTORY_GESTURE");

            AddValidConstant(LSLType.Integer, "INVENTORY_LANDMARK");

            AddValidConstant(LSLType.Integer, "INVENTORY_NONE");

            AddValidConstant(LSLType.Integer, "INVENTORY_NOTECARD");

            AddValidConstant(LSLType.Integer, "INVENTORY_OBJECT");

            AddValidConstant(LSLType.Integer, "INVENTORY_SCRIPT");

            AddValidConstant(LSLType.Integer, "INVENTORY_SOUND");

            AddValidConstant(LSLType.Integer, "INVENTORY_TEXTURE");

            AddValidConstant(LSLType.Integer, "JSON_APPEND");

            AddValidConstant(LSLType.String, "JSON_ARRAY");

            AddValidConstant(LSLType.String, "JSON_DELETE");

            AddValidConstant(LSLType.String, "JSON_FALSE");

            AddValidConstant(LSLType.String, "JSON_INVALID");

            AddValidConstant(LSLType.String, "JSON_NULL");

            AddValidConstant(LSLType.String, "JSON_NUMBER");

            AddValidConstant(LSLType.String, "JSON_OBJECT");

            AddValidConstant(LSLType.String, "JSON_STRING");

            AddValidConstant(LSLType.String, "JSON_TRUE");

            AddValidConstant(LSLType.Integer, "KFM_CMD_PAUSE");

            AddValidConstant(LSLType.Integer, "KFM_CMD_PLAY");

            AddValidConstant(LSLType.Integer, "KFM_CMD_STOP");

            AddValidConstant(LSLType.Integer, "KFM_COMMAND");

            AddValidConstant(LSLType.Integer, "KFM_DATA");

            AddValidConstant(LSLType.Integer, "KFM_FORWARD");

            AddValidConstant(LSLType.Integer, "KFM_LOOP");

            AddValidConstant(LSLType.Integer, "KFM_MODE");

            AddValidConstant(LSLType.Integer, "KFM_PING_PONG");

            AddValidConstant(LSLType.Integer, "KFM_REVERSE");

            AddValidConstant(LSLType.Integer, "KFM_ROTATION");

            AddValidConstant(LSLType.Integer, "KFM_TRANSLATION");

            AddValidConstant(LSLType.Integer, "LAND_LEVEL");

            AddValidConstant(LSLType.Integer, "LAND_LOWER");

            AddValidConstant(LSLType.Integer, "LAND_NOISE");

            AddValidConstant(LSLType.Integer, "LAND_RAISE");

            AddValidConstant(LSLType.Integer, "LAND_REVERT");

            AddValidConstant(LSLType.Integer, "LAND_SMOOTH");

            AddValidConstant(LSLType.Integer, "LINK_ALL_CHILDREN");

            AddValidConstant(LSLType.Integer, "LINK_ALL_OTHERS");

            AddValidConstant(LSLType.Integer, "LINK_ROOT");

            AddValidConstant(LSLType.Integer, "LINK_SET");

            AddValidConstant(LSLType.Integer, "LINK_THIS");

            AddValidConstant(LSLType.Integer, "LIST_STAT_GEOMETRIC_MEAN");

            AddValidConstant(LSLType.Integer, "LIST_STAT_MAX");

            AddValidConstant(LSLType.Integer, "LIST_STAT_MEAN");

            AddValidConstant(LSLType.Integer, "LIST_STAT_MEDIAN");

            AddValidConstant(LSLType.Integer, "LIST_STAT_MIN");

            AddValidConstant(LSLType.Integer, "LIST_STAT_NUM_COUNT");

            AddValidConstant(LSLType.Integer, "LIST_STAT_RANGE");

            AddValidConstant(LSLType.Integer, "LIST_STAT_STD_DEV");

            AddValidConstant(LSLType.Integer, "LIST_STAT_SUM");

            AddValidConstant(LSLType.Integer, "LIST_STAT_SUM_SQUARES");

            AddValidConstant(LSLType.Integer, "LOOP");

            AddValidConstant(LSLType.Integer, "MASK_BASE");

            AddValidConstant(LSLType.Integer, "MASK_EVERYONE");

            AddValidConstant(LSLType.Integer, "MASK_GROUP");

            AddValidConstant(LSLType.Integer, "MASK_NEXT");

            AddValidConstant(LSLType.Integer, "MASK_OWNER");

            AddValidConstant(LSLType.String, "NULL_KEY");

            AddValidConstant(LSLType.Integer, "OBJECT_ATTACHED_POINT");

            AddValidConstant(LSLType.Integer, "OBJECT_CHARACTER_TIME");

            AddValidConstant(LSLType.Integer, "OBJECT_CREATOR");

            AddValidConstant(LSLType.Integer, "OBJECT_DESC");

            AddValidConstant(LSLType.Integer, "OBJECT_GROUP");

            AddValidConstant(LSLType.Integer, "OBJECT_NAME");

            AddValidConstant(LSLType.Integer, "OBJECT_OWNER");

            AddValidConstant(LSLType.Integer, "OBJECT_PATHFINDING_TYPE");

            AddValidConstant(LSLType.Integer, "OBJECT_PHANTOM");

            AddValidConstant(LSLType.Integer, "OBJECT_PHYSICS");

            AddValidConstant(LSLType.Integer, "OBJECT_PHYSICS_COST");

            AddValidConstant(LSLType.Integer, "OBJECT_POS");

            AddValidConstant(LSLType.Integer, "OBJECT_PRIM_EQUIVALENCE");

            AddValidConstant(LSLType.Integer, "OBJECT_RENDER_WEIGHT");

            AddValidConstant(LSLType.Integer, "OBJECT_RETURN_PARCEL");

            AddValidConstant(LSLType.Integer, "OBJECT_RETURN_PARCEL_OWNER");

            AddValidConstant(LSLType.Integer, "OBJECT_RETURN_REGION");

            AddValidConstant(LSLType.Integer, "OBJECT_ROOT");

            AddValidConstant(LSLType.Integer, "OBJECT_ROT");

            AddValidConstant(LSLType.Integer, "OBJECT_RUNNING_SCRIPT_COUNT");

            AddValidConstant(LSLType.Integer, "OBJECT_SCRIPT_MEMORY");

            AddValidConstant(LSLType.Integer, "OBJECT_SCRIPT_TIME");

            AddValidConstant(LSLType.Integer, "OBJECT_SERVER_COST");

            AddValidConstant(LSLType.Integer, "OBJECT_STREAMING_COST");

            AddValidConstant(LSLType.Integer, "OBJECT_TEMP_ON_REZ");

            AddValidConstant(LSLType.Integer, "OBJECT_TOTAL_SCRIPT_COUNT");

            AddValidConstant(LSLType.Integer, "OBJECT_UNKNOWN_DETAIL");

            AddValidConstant(LSLType.Integer, "OBJECT_VELOCITY");

            AddValidConstant(LSLType.Integer, "OPT_CHARACTER");

            AddValidConstant(LSLType.Integer, "OPT_AVATAR");

            AddValidConstant(LSLType.Integer, "OPT_EXCLUSION_VOLUME");

            AddValidConstant(LSLType.Integer, "OPT_LEGACY_LINKSET");

            AddValidConstant(LSLType.Integer, "OPT_MATERIAL_VOLUME");

            AddValidConstant(LSLType.Integer, "OPT_OTHER");

            AddValidConstant(LSLType.Integer, "OPT_STATIC_OBSTACLE");

            AddValidConstant(LSLType.Integer, "OPT_WALKABLE");

            AddValidConstant(LSLType.Integer, "PARCEL_COUNT_GROUP");

            AddValidConstant(LSLType.Integer, "PARCEL_COUNT_OTHER");

            AddValidConstant(LSLType.Integer, "PARCEL_COUNT_OWNER");

            AddValidConstant(LSLType.Integer, "PARCEL_COUNT_SELECTED");

            AddValidConstant(LSLType.Integer, "PARCEL_COUNT_TEMP");

            AddValidConstant(LSLType.Integer, "PARCEL_COUNT_TOTAL");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_AREA");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_DESC");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_GROUP");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_ID");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_NAME");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_OWNER");

            AddValidConstant(LSLType.Integer, "PARCEL_DETAILS_SEE_AVATARS");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_ALL_OBJECT_ENTRY");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_CREATE_GROUP_OBJECTS");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_CREATE_OBJECTS");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_DAMAGE");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_FLY");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_GROUP_OBJECT_ENTRY");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_GROUP_SCRIPTS");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_LANDMARK");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_SCRIPTS");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_ALLOW_TERRAFORM");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_LOCAL_SOUND_ONLY");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_RESTRICT_PUSHOBJECT");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_USE_ACCESS_GROUP");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_USE_ACCESS_LIST");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_USE_BAN_LIST");

            AddValidConstant(LSLType.Integer, "PARCEL_FLAG_USE_LAND_PASS_LIST");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_AGENT");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_AUTO_ALIGN");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_DESC");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_LOOP");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_LOOP_SET");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_PAUSE");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_PLAY");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_SIZE");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_STOP");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_TEXTURE");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_TIME");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_TYPE");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_UNLOAD");

            AddValidConstant(LSLType.Integer, "PARCEL_MEDIA_COMMAND_URL");

            AddValidConstant(LSLType.Integer, "PASSIVE");

            AddValidConstant(LSLType.Integer, "PATROL_PAUSE_AT_WAYPOINTS");

            AddValidConstant(LSLType.Integer, "PAYMENT_INFO_ON_FILE");

            AddValidConstant(LSLType.Integer, "PAYMENT_INFO_USED");

            AddValidConstant(LSLType.Integer, "PAY_DEFAULT");

            AddValidConstant(LSLType.Integer, "PAY_HIDE");

            AddValidConstant(LSLType.Integer, "PERMISSION_ATTACH");

            AddValidConstant(LSLType.Integer, "PERMISSION_CHANGE_LINKS");

            AddValidConstant(LSLType.Integer, "PERMISSION_CONTROL_CAMERA");

            AddValidConstant(LSLType.Integer, "PERMISSION_DEBIT");

            AddValidConstant(LSLType.Integer, "PERMISSION_OVERRIDE_ANIMATIONS");

            AddValidConstant(LSLType.Integer, "PERMISSION_RETURN_OBJECTS");

            AddValidConstant(LSLType.Integer, "PERMISSION_SILENT_ESTATE_MANAGEMENT");

            AddValidConstant(LSLType.Integer, "PERMISSION_TAKE_CONTROLS");

            AddValidConstant(LSLType.Integer, "PERMISSION_TELEPORT");

            AddValidConstant(LSLType.Integer, "PERMISSION_TRACK_CAMERA");

            AddValidConstant(LSLType.Integer, "PERMISSION_TRIGGER_ANIMATION");

            AddValidConstant(LSLType.Integer, "PERM_ALL");

            AddValidConstant(LSLType.Integer, "PERM_COPY");

            AddValidConstant(LSLType.Integer, "PERM_MODIFY");

            AddValidConstant(LSLType.Integer, "PERM_MOVE");

            AddValidConstant(LSLType.Integer, "PERM_TRANSFER");

            AddValidConstant(LSLType.Float, "PI");

            AddValidConstant(LSLType.Integer, "PING_PONG");

            AddValidConstant(LSLType.Float, "PI_BY_TWO");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_BARK");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_BLOBS");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_BRICKS");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_BRIGHT");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_CHECKER");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_CONCRETE");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_DARK");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_DISKS");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_GRAVEL");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_LARGETILE");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_NONE");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_SHINY");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_SIDING");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_STONE");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_STUCCO");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_SUCTION");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_TILE");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_WEAVE");

            AddValidConstant(LSLType.Integer, "PRIM_BUMP_WOOD");

            AddValidConstant(LSLType.Integer, "PRIM_COLOR");

            AddValidConstant(LSLType.Integer, "PRIM_DESC");

            AddValidConstant(LSLType.Integer, "PRIM_FLEXIBLE");

            AddValidConstant(LSLType.Integer, "PRIM_FULLBRIGHT");

            AddValidConstant(LSLType.Integer, "PRIM_GLOW");

            AddValidConstant(LSLType.Integer, "PRIM_HOLE_CIRCLE");

            AddValidConstant(LSLType.Integer, "PRIM_HOLE_DEFAULT");

            AddValidConstant(LSLType.Integer, "PRIM_HOLE_SQUARE");

            AddValidConstant(LSLType.Integer, "PRIM_HOLE_TRIANGLE");

            AddValidConstant(LSLType.Integer, "PRIM_LINK_TARGET");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_FLESH");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_GLASS");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_LIGHT");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_METAL");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_PLASTIC");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_RUBBER");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_STONE");

            AddValidConstant(LSLType.Integer, "PRIM_MATERIAL_WOOD");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_ALT_IMAGE_ENABLE");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_AUTO_LOOP");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_AUTO_PLAY");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_AUTO_SCALE");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_AUTO_ZOOM");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_CURRENT_URL");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_FIRST_CLICK_INTERACT");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_HEIGHT_PIXELS");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_HOME_URL");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_PERMS_CONTROL");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_PERMS_INTERACT");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_PERM_ANYONE");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_PERM_GROUP");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_PERM_NONE");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_PERM_OWNER");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_WHITELIST");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_WHITELIST_ENABLE");

            AddValidConstant(LSLType.Integer, "PRIM_MEDIA_WIDTH_PIXELS");

            AddValidConstant(LSLType.Integer, "PRIM_NAME");

            AddValidConstant(LSLType.Integer, "PRIM_OMEGA");

            AddValidConstant(LSLType.Integer, "PRIM_PHANTOM");

            AddValidConstant(LSLType.Integer, "PRIM_PHYSICS");

            AddValidConstant(LSLType.Integer, "PRIM_PHYSICS_SHAPE_CONVEX");

            AddValidConstant(LSLType.Integer, "PRIM_PHYSICS_SHAPE_NONE");

            AddValidConstant(LSLType.Integer, "PRIM_PHYSICS_SHAPE_PRIM");

            AddValidConstant(LSLType.Integer, "PRIM_PHYSICS_SHAPE_TYPE");

            AddValidConstant(LSLType.Integer, "PRIM_POINT_LIGHT");

            AddValidConstant(LSLType.Integer, "PRIM_POSITION");

            AddValidConstant(LSLType.Integer, "PRIM_POS_LOCAL");

            AddValidConstant(LSLType.Integer, "PRIM_ROTATION");

            AddValidConstant(LSLType.Integer, "PRIM_ROT_LOCAL");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_FLAG_INVERT");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_FLAG_MIRROR");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_TYPE_CYLINDER");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_TYPE_MASK");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_TYPE_PLANE");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_TYPE_SPHERE");

            AddValidConstant(LSLType.Integer, "PRIM_SCULPT_TYPE_TORUS");

            AddValidConstant(LSLType.Integer, "PRIM_SHINY_HIGH");

            AddValidConstant(LSLType.Integer, "PRIM_SHINY_LOW");

            AddValidConstant(LSLType.Integer, "PRIM_SHINY_MEDIUM");

            AddValidConstant(LSLType.Integer, "PRIM_SHINY_NONE");

            AddValidConstant(LSLType.Integer, "PRIM_SIZE");

            AddValidConstant(LSLType.Integer, "PRIM_SLICE");

            AddValidConstant(LSLType.Integer, "PRIM_TEMP_ON_REZ");

            AddValidConstant(LSLType.Integer, "PRIM_TEXGEN");

            AddValidConstant(LSLType.Integer, "PRIM_TEXGEN_DEFAULT");

            AddValidConstant(LSLType.Integer, "PRIM_TEXGEN_PLANAR");

            AddValidConstant(LSLType.Integer, "PRIM_TEXT");

            AddValidConstant(LSLType.Integer, "PRIM_TEXTURE");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_BOX");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_CYLINDER");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_PRISM");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_RING");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_SCULPT");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_SPHERE");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_TORUS");

            AddValidConstant(LSLType.Integer, "PRIM_TYPE_TUBE");

            AddValidConstant(LSLType.Integer, "PROFILE_NONE");

            AddValidConstant(LSLType.Integer, "PROFILE_SCRIPT_MEMORY");

            AddValidConstant(LSLType.Integer, "PUBLIC_CHANNEL");

            AddValidConstant(LSLType.Float, "RAD_TO_DEG");

            AddValidConstant(LSLType.Integer, "RCERR_CAST_TIME_EXCEEDED");

            AddValidConstant(LSLType.Integer, "RCERR_SIM_PERF_LOW");

            AddValidConstant(LSLType.Integer, "RCERR_UNKNOWN");

            AddValidConstant(LSLType.Integer, "RC_DATA_FLAGS");

            AddValidConstant(LSLType.Integer, "RC_DETECT_PHANTOM");

            AddValidConstant(LSLType.Integer, "RC_GET_LINK_NUM");

            AddValidConstant(LSLType.Integer, "RC_GET_NORMAL");

            AddValidConstant(LSLType.Integer, "RC_GET_ROOT_KEY");

            AddValidConstant(LSLType.Integer, "RC_MAX_HITS");

            AddValidConstant(LSLType.Integer, "RC_REJECT_AGENTS");

            AddValidConstant(LSLType.Integer, "RC_REJECT_LAND");

            AddValidConstant(LSLType.Integer, "RC_REJECT_NONPHYSICAL");

            AddValidConstant(LSLType.Integer, "RC_REJECT_PHYSICAL");

            AddValidConstant(LSLType.Integer, "RC_REJECT_TYPES");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_ALLOW_DAMAGE");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_ALLOW_DIRECT_TELEPORT");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_BLOCK_FLY");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_BLOCK_TERRAFORM");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_DISABLE_COLLISIONS");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_DISABLE_PHYSICS");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_FIXED_SUN");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_RESTRICT_PUSHOBJECT");

            AddValidConstant(LSLType.Integer, "REGION_FLAG_SANDBOX");

            AddValidConstant(LSLType.Integer, "REMOTE_DATA_CHANNEL");

            AddValidConstant(LSLType.Integer, "REMOTE_DATA_REPLY");

            AddValidConstant(LSLType.Integer, "REMOTE_DATA_REQUEST");

            AddValidConstant(LSLType.Integer, "REVERSE");

            AddValidConstant(LSLType.Integer, "ROTATE");

            AddValidConstant(LSLType.Integer, "SCALE");

            AddValidConstant(LSLType.Integer, "SCRIPTED");

            AddValidConstant(LSLType.Integer, "SIM_STAT_PCT_CHARS_STEPPED");

            AddValidConstant(LSLType.Integer, "SMOOTH");

            AddValidConstant(LSLType.Float, "SQRT2");

            AddValidConstant(LSLType.Integer, "STATUS_BLOCK_GRAB");

            AddValidConstant(LSLType.Integer, "STATUS_BLOCK_GRAB_OBJECT");

            AddValidConstant(LSLType.Integer, "STATUS_BOUNDS_ERROR");

            AddValidConstant(LSLType.Integer, "STATUS_CAST_SHADOWS");

            AddValidConstant(LSLType.Integer, "STATUS_DIE_AT_EDGE");

            AddValidConstant(LSLType.Integer, "STATUS_INTERNAL_ERROR");

            AddValidConstant(LSLType.Integer, "STATUS_MALFORMED_PARAMS");

            AddValidConstant(LSLType.Integer, "STATUS_NOT_FOUND");

            AddValidConstant(LSLType.Integer, "STATUS_NOT_SUPPORTED");

            AddValidConstant(LSLType.Integer, "STATUS_OK");

            AddValidConstant(LSLType.Integer, "STATUS_PHANTOM");

            AddValidConstant(LSLType.Integer, "STATUS_PHYSICS");

            AddValidConstant(LSLType.Integer, "STATUS_RETURN_AT_EDGE");

            AddValidConstant(LSLType.Integer, "STATUS_ROTATE_X");

            AddValidConstant(LSLType.Integer, "STATUS_ROTATE_Y");

            AddValidConstant(LSLType.Integer, "STATUS_ROTATE_Z");

            AddValidConstant(LSLType.Integer, "STATUS_SANDBOX");

            AddValidConstant(LSLType.Integer, "STATUS_TYPE_MISMATCH");

            AddValidConstant(LSLType.Integer, "STATUS_WHITELIST_FAILED");

            AddValidConstant(LSLType.Integer, "STRING_TRIM");

            AddValidConstant(LSLType.Integer, "STRING_TRIM_HEAD");

            AddValidConstant(LSLType.Integer, "STRING_TRIM_TAIL");

            AddValidConstant(LSLType.String, "TEXTURE_PLYWOOD");

            AddValidConstant(LSLType.String, "TEXTURE_DEFAULT");

            AddValidConstant(LSLType.String, "TEXTURE_BLANK");

            AddValidConstant(LSLType.String, "TEXTURE_MEDIA");

            AddValidConstant(LSLType.String, "TEXTURE_TRANSPARENT");

            AddValidConstant(LSLType.Integer, "TOUCH_INVALID_FACE");

            AddValidConstant(LSLType.Vector, "TOUCH_INVALID_TEXCOORD");

            AddValidConstant(LSLType.Vector, "TOUCH_INVALID_VECTOR");

            AddValidConstant(LSLType.Integer, "TRAVERSAL_TYPE");

            AddValidConstant(LSLType.Integer, "TRUE");

            AddValidConstant(LSLType.Float, "TWO_PI");

            AddValidConstant(LSLType.Integer, "TYPE_FLOAT");

            AddValidConstant(LSLType.Integer, "TYPE_INTEGER");

            AddValidConstant(LSLType.Integer, "TYPE_INVALID");

            AddValidConstant(LSLType.Integer, "TYPE_KEY");

            AddValidConstant(LSLType.Integer, "TYPE_ROTATION");

            AddValidConstant(LSLType.Integer, "TYPE_STRING");

            AddValidConstant(LSLType.Integer, "TYPE_VECTOR");

            AddValidConstant(LSLType.String, "URL_REQUEST_DENIED");

            AddValidConstant(LSLType.String, "URL_REQUEST_GRANTED");

            AddValidConstant(LSLType.Integer, "VEHICLE_ANGULAR_DEFLECTION_EFFICIENCY");

            AddValidConstant(LSLType.Integer, "VEHICLE_ANGULAR_DEFLECTION_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_ANGULAR_FRICTION_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_ANGULAR_MOTOR_DECAY_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_ANGULAR_MOTOR_DIRECTION");

            AddValidConstant(LSLType.Integer, "VEHICLE_ANGULAR_MOTOR_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_BANKING_EFFICIENCY");

            AddValidConstant(LSLType.Integer, "VEHICLE_BANKING_MIX");

            AddValidConstant(LSLType.Integer, "VEHICLE_BANKING_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_BUOYANCY");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_CAMERA_DECOUPLED");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_HOVER_GLOBAL_HEIGHT");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_HOVER_TERRAIN_ONLY");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_HOVER_UP_ONLY");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_HOVER_WATER_ONLY");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_LIMIT_MOTOR_UP");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_LIMIT_ROLL_ONLY");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_MOUSELOOK_BANK");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_MOUSELOOK_STEER");

            AddValidConstant(LSLType.Integer, "VEHICLE_FLAG_NO_DEFLECTION_UP");

            AddValidConstant(LSLType.Integer, "VEHICLE_HOVER_EFFICIENCY");

            AddValidConstant(LSLType.Integer, "VEHICLE_HOVER_HEIGHT");

            AddValidConstant(LSLType.Integer, "VEHICLE_HOVER_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_DEFLECTION_EFFICIENCY");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_DEFLECTION_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_FRICTION_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_MOTOR_DECAY_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_MOTOR_DIRECTION");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_MOTOR_OFFSET");

            AddValidConstant(LSLType.Integer, "VEHICLE_LINEAR_MOTOR_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VEHICLE_REFERENCE_FRAME");

            AddValidConstant(LSLType.Integer, "VEHICLE_TYPE_AIRPLANE");

            AddValidConstant(LSLType.Integer, "VEHICLE_TYPE_BALLOON");

            AddValidConstant(LSLType.Integer, "VEHICLE_TYPE_BOAT");

            AddValidConstant(LSLType.Integer, "VEHICLE_TYPE_CAR");

            AddValidConstant(LSLType.Integer, "VEHICLE_TYPE_NONE");

            AddValidConstant(LSLType.Integer, "VEHICLE_TYPE_SLED");

            AddValidConstant(LSLType.Integer, "VEHICLE_VERTICAL_ATTRACTION_EFFICIENCY");

            AddValidConstant(LSLType.Integer, "VEHICLE_VERTICAL_ATTRACTION_TIMESCALE");

            AddValidConstant(LSLType.Integer, "VERTICAL");

            AddValidConstant(LSLType.Rotation, "ZERO_ROTATION");

            AddValidConstant(LSLType.Vector, "ZERO_VECTOR");
            */


            #endregion
        }



        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
        }


        /// <summary>
        ///     This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return
        ///     null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the
        ///     <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        ///     produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
        ///     and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
        ///     method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }




        /// <summary>
        ///     Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        public void ReadXml(XmlReader reader)
        {
            var lineInfo = (IXmlLineInfo) reader;
            try
            {
                var canRead = reader.Read();
                while (canRead)
                {
                    if (reader.Name == "LibraryFunction" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryFunctionSignature.FromXmlFragment(reader);


                        if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                        {
                            AddValidLibraryFunction(signature);

                        }


                        canRead = reader.Read();
                    }
                    else if (reader.Name == "EventHandler" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryEventSignature.FromXmlFragment(reader);


                        if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                        {
                            AddValidEventHandler(signature);
                        }
                        

                        canRead = reader.Read();
                    }
                    else if (reader.Name == "LibraryConstant" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryConstantSignature.FromXmlFragment(reader);


                        if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                        {
                            AddValidConstant(signature);
                        }
                        

                        canRead = reader.Read();
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "LSLLibraryData")
                    {
                        break;
                    }
                    else
                    {
                        canRead = reader.Read();
                    }
                }
            }
            catch (LSLDuplicateSignatureException e)
            {
                throw new XmlSyntaxException(lineInfo.LineNumber,e.Message);
            }
        }



        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var func in LibraryFunctions.SelectMany(x=>x))
            {
                writer.WriteStartElement("LibraryFunction");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in SupportedEventHandlers)
            {
                writer.WriteStartElement("EventHandler");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in LibraryConstants)
            {
                writer.WriteStartElement("LibraryConstant");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }
        }



        /// <summary>
        ///     Fills a library data provider from an XML reader object
        /// </summary>
        /// <param name="data">The xml reader to read from</param>
        /// <param name="subsets">
        /// Data nodes must contain one of these subset strings in their Subsets property, otherwise they are discarded. 
        /// when "all" is used, all nodes are added and duplicates are accumulated into DuplicateEventsDefined, DuplicateConstantsDefined
        /// and DuplicateFunctionsDefined</param>
        /// <exception cref="ArgumentNullException">When data is null</exception>
        /// <exception cref="XmlException">When a syntax error is encountered</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void FillFromXml(XmlReader data, IReadOnlySet<string> subsets)
        {


           
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            ClearEventHandlers();
            ClearLibraryConstants();
            ClearLibraryFunctions();

            _subsets=new HashSet<string>(subsets);

            if (_subsets.Contains("all"))
            {
                AccumulateDuplicates = true;
            }

            data.ReadStartElement(LSLXmlLibraryDataRootAttribute.RootElementName);

            ReadXml(data);

            data.ReadEndElement();
        }
    }
}