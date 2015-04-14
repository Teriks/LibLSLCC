using System;

namespace LibLSLCC.LSLRuntime
{
    public class LSLScriptBase : LSLScriptLibraryBase
    {
        public override void at_target(LSL_Types.LSLInteger tnum, LSL_Types.Vector3 targetpos, LSL_Types.Vector3 ourpos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void control(LSL_Types.key id, LSL_Types.LSLInteger level, LSL_Types.LSLInteger edge)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void at_rot_target(LSL_Types.LSLInteger handle, LSL_Types.Quaternion targetrot,
            LSL_Types.Quaternion ourrot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void not_at_rot_target()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void not_at_target()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void http_response(LSL_Types.key request_id, LSL_Types.LSLInteger status,
            LSL_Types.list metadata, LSL_Types.LSLString body)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void on_rez(LSL_Types.LSLInteger start_param)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void attach(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void land_collision(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void path_update(LSL_Types.LSLInteger type, LSL_Types.list reserved)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void changed(LSL_Types.LSLInteger change)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void collision(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void collision_start(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void collision_end(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void land_collision_end(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void land_collision_start(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void link_message(LSL_Types.LSLInteger sender_num, LSL_Types.LSLInteger num,
            LSL_Types.LSLString str, LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void listen(LSL_Types.LSLInteger channel, LSL_Types.LSLString name, LSL_Types.key id,
            LSL_Types.LSLString message)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void remote_data(LSL_Types.LSLInteger event_type, LSL_Types.key channel,
            LSL_Types.key message_id, LSL_Types.LSLString sender, LSL_Types.LSLInteger idata, LSL_Types.LSLString sdata)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void run_time_permissions(LSL_Types.LSLInteger perm)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void sensor(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void state_entry()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void timer()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void touch(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void touch_end(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void touch_start(LSL_Types.LSLInteger num_detected)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void transaction_result(LSL_Types.key id, LSL_Types.LSLInteger success, LSL_Types.LSLString data)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void no_sensor()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void email(LSL_Types.LSLString time, LSL_Types.LSLString address, LSL_Types.LSLString subject,
            LSL_Types.LSLString message, LSL_Types.LSLInteger num_left)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void http_request(LSL_Types.key request_id, LSL_Types.LSLString method, LSL_Types.LSLString body)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void object_rez(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void money(LSL_Types.key id, LSL_Types.LSLInteger amount)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void moving_end()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void moving_start()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void state_exit()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLinkSitTarget(LSL_Types.LSLInteger link, LSL_Types.Vector3 offset,
            LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llList2CSV(LSL_Types.list src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llList2Float(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llList2Integer(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llList2Json(LSL_Types.LSLString type, LSL_Types.list values)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llList2Key(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llList2List(LSL_Types.list src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger end)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llList2ListStrided(LSL_Types.list src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger end, LSL_Types.LSLInteger stride)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llList2Rot(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llList2String(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llList2Vector(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llListen(LSL_Types.LSLInteger channel, LSL_Types.LSLString name,
            LSL_Types.key id, LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llListenControl(LSL_Types.LSLInteger handle, LSL_Types.LSLInteger active)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llListenRemove(LSL_Types.LSLInteger handle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llListFindList(LSL_Types.list src, LSL_Types.list test)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llListInsertList(LSL_Types.list dest, LSL_Types.list src,
            LSL_Types.LSLInteger start)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llListRandomize(LSL_Types.list src, LSL_Types.LSLInteger stride)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llListReplaceList(LSL_Types.list dest, LSL_Types.list src,
            LSL_Types.LSLInteger start, LSL_Types.LSLInteger end)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llListSort(LSL_Types.list src, LSL_Types.LSLInteger stride,
            LSL_Types.LSLInteger ascending)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llListStatistics(LSL_Types.LSLInteger operation, LSL_Types.list src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLoadURL(LSL_Types.key avatar, LSL_Types.LSLString message, LSL_Types.LSLString url)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llLog(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llLog10(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLookAt(LSL_Types.Vector3 target, LSL_Types.LSLFloat strength, LSL_Types.LSLFloat damping)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLoopSound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLoopSoundMaster(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLoopSoundSlave(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMakeExplosion(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale,
            LSL_Types.LSLFloat vel, LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLString texture,
            LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMakeFire(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale, LSL_Types.LSLFloat vel,
            LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLString texture, LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMakeFountain(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale,
            LSL_Types.LSLFloat vel, LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLInteger bounce,
            LSL_Types.LSLString texture, LSL_Types.Vector3 offset, LSL_Types.LSLFloat bounce_offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMakeSmoke(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale,
            LSL_Types.LSLFloat vel, LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLString texture,
            LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llManageEstateAccess(LSL_Types.LSLInteger action, LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMapDestination(LSL_Types.LSLString simname, LSL_Types.Vector3 pos,
            LSL_Types.Vector3 look_at)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llMD5String(LSL_Types.LSLString src, LSL_Types.LSLInteger nonce)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMessageLinked(LSL_Types.LSLInteger link, LSL_Types.LSLInteger num,
            LSL_Types.LSLString str, LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMinEventDelay(LSL_Types.LSLFloat delay)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llModifyLand(LSL_Types.LSLInteger action, LSL_Types.LSLInteger brush)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llModPow(LSL_Types.LSLInteger a, LSL_Types.LSLInteger b,
            LSL_Types.LSLInteger c)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llMoveToTarget(LSL_Types.Vector3 target, LSL_Types.LSLFloat tau)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llNavigateTo(LSL_Types.Vector3 pos, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llOffsetTexture(LSL_Types.LSLFloat u, LSL_Types.LSLFloat v, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llOpenRemoteDataChannel()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llOverMyLand(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llOwnerSay(LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llParcelMediaCommandList(LSL_Types.list commandList)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llParcelMediaQuery(LSL_Types.list query)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llParseString2List(LSL_Types.LSLString src, LSL_Types.list separators,
            LSL_Types.list spacers)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llParseStringKeepNulls(LSL_Types.LSLString src, LSL_Types.list separators,
            LSL_Types.list spacers)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llParticleSystem(LSL_Types.list rules)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llLinkParticleSystem(LSL_Types.LSLInteger link, LSL_Types.list rules)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPassCollisions(LSL_Types.LSLInteger pass)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPassTouches(LSL_Types.LSLInteger pass)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPatrolPoints(LSL_Types.list patrolPoints, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPlaySound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPlaySoundSlave(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPointAt(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llPow(LSL_Types.LSLFloat base_number, LSL_Types.LSLFloat exponent)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPreloadSound(LSL_Types.LSLString sound)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPursue(LSL_Types.key target, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llPushObject(LSL_Types.key target, LSL_Types.Vector3 impulse, LSL_Types.Vector3 ang_impulse,
            LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRefreshPrimURL()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRegionSay(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRegionSayTo(LSL_Types.key target, LSL_Types.LSLInteger channel, LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llReleaseCamera(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llReleaseControls()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llReleaseURL(LSL_Types.LSLString url)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoteDataReply(LSL_Types.key channel, LSL_Types.key message_id,
            LSL_Types.LSLString sdata, LSL_Types.LSLInteger idata)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoteDataSetRegion()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoteLoadScript(LSL_Types.key target, LSL_Types.LSLString name,
            LSL_Types.LSLInteger running, LSL_Types.LSLInteger start_param)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoteLoadScriptPin(LSL_Types.key target, LSL_Types.LSLString name,
            LSL_Types.LSLInteger pin, LSL_Types.LSLInteger running, LSL_Types.LSLInteger start_param)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoveFromLandBanList(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoveFromLandPassList(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoveInventory(LSL_Types.LSLString item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRemoveVehicleFlags(LSL_Types.LSLInteger flags)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestAgentData(LSL_Types.key id, LSL_Types.LSLInteger data)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestDisplayName(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestInventoryData(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRequestPermissions(LSL_Types.key agent, LSL_Types.LSLInteger permissions)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestSecureURL()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestSimulatorData(LSL_Types.LSLString region, LSL_Types.LSLInteger data)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestURL()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llRequestUsername(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llResetAnimationOverride(LSL_Types.LSLString anim_state)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llResetLandBanList()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llResetLandPassList()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llResetOtherScript(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llResetScript()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llResetTime()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llReturnObjectsByID(LSL_Types.list objects)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llReturnObjectsByOwner(LSL_Types.key owner, LSL_Types.LSLInteger scope)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRezAtRoot(LSL_Types.LSLString inventory, LSL_Types.Vector3 position,
            LSL_Types.Vector3 velocity, LSL_Types.Quaternion rot, LSL_Types.LSLInteger param)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRezObject(LSL_Types.LSLString inventory, LSL_Types.Vector3 pos, LSL_Types.Vector3 vel,
            LSL_Types.Quaternion rot, LSL_Types.LSLInteger param)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llRot2Angle(LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llRot2Axis(LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llRot2Euler(LSL_Types.Quaternion quat)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llRot2Fwd(LSL_Types.Quaternion q)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llRot2Left(LSL_Types.Quaternion q)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llRot2Up(LSL_Types.Quaternion q)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRotateTexture(LSL_Types.LSLFloat angle, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llRotBetween(LSL_Types.Vector3 start, LSL_Types.Vector3 end)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRotLookAt(LSL_Types.Quaternion target_direction, LSL_Types.LSLFloat strength,
            LSL_Types.LSLFloat damping)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llRotTarget(LSL_Types.Quaternion rot, LSL_Types.LSLFloat error)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llRotTargetRemove(LSL_Types.LSLInteger handle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llRound(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llSameGroup(LSL_Types.key uuid)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSay(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llScaleByFactor(LSL_Types.LSLFloat scaling_factor)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llScaleTexture(LSL_Types.LSLFloat u, LSL_Types.LSLFloat v, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llScriptDanger(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llScriptProfiler(LSL_Types.LSLInteger flags)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llSendRemoteData(LSL_Types.key channel, LSL_Types.LSLString dest,
            LSL_Types.LSLInteger idata, LSL_Types.LSLString sdata)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSensor(LSL_Types.LSLString name, LSL_Types.key id, LSL_Types.LSLInteger type,
            LSL_Types.LSLFloat range, LSL_Types.LSLFloat arc)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSensorRemove()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSensorRepeat(LSL_Types.LSLString name, LSL_Types.key id, LSL_Types.LSLInteger type,
            LSL_Types.LSLFloat range, LSL_Types.LSLFloat arc, LSL_Types.LSLFloat rate)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetAlpha(LSL_Types.LSLFloat alpha, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetAngularVelocity(LSL_Types.Vector3 initial_omega, LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetAnimationOverride(LSL_Types.LSLString anim_state, LSL_Types.LSLString anim)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetBuoyancy(LSL_Types.LSLFloat buoyancy)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetCameraAtOffset(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetCameraEyeOffset(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetCameraParams(LSL_Types.list rules)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetClickAction(LSL_Types.LSLInteger action)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetColor(LSL_Types.Vector3 color, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetContentType(LSL_Types.key request_id, LSL_Types.LSLInteger content_type)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetDamage(LSL_Types.LSLFloat damage)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetForce(LSL_Types.Vector3 force, LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetForceAndTorque(LSL_Types.Vector3 force, LSL_Types.Vector3 torque,
            LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetHoverHeight(LSL_Types.LSLFloat height, LSL_Types.LSLInteger water,
            LSL_Types.LSLFloat tau)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetInventoryPermMask(LSL_Types.LSLString item, LSL_Types.LSLInteger category,
            LSL_Types.LSLInteger value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetKeyframedMotion(LSL_Types.list keyframes, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkAlpha(LSL_Types.LSLInteger link, LSL_Types.LSLFloat alpha,
            LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkCamera(LSL_Types.LSLInteger link, LSL_Types.Vector3 eye, LSL_Types.Vector3 at)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkColor(LSL_Types.LSLInteger link, LSL_Types.Vector3 color,
            LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llSetLinkMedia(LSL_Types.LSLInteger link, LSL_Types.LSLInteger face,
            LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkTexture(LSL_Types.LSLInteger link, LSL_Types.LSLString texture,
            LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkTextureAnim(LSL_Types.LSLInteger link, LSL_Types.LSLInteger mode,
            LSL_Types.LSLInteger face, LSL_Types.LSLInteger sizex, LSL_Types.LSLInteger sizey, LSL_Types.LSLFloat start,
            LSL_Types.LSLFloat length, LSL_Types.LSLFloat rate)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLocalRot(LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llSetMemoryLimit(LSL_Types.LSLInteger limit)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetObjectDesc(LSL_Types.LSLString description)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetObjectName(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetObjectPermMask(LSL_Types.LSLInteger mask, LSL_Types.LSLInteger value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetParcelMusicURL(LSL_Types.LSLString url)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetPayPrice(LSL_Types.LSLInteger price, LSL_Types.list quick_pay_buttons)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetPhysicsMaterial(LSL_Types.LSLInteger mask, LSL_Types.LSLFloat gravity_multiplier,
            LSL_Types.LSLFloat restitution, LSL_Types.LSLFloat friction, LSL_Types.LSLFloat density)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetPos(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetPrimitiveParams(LSL_Types.list rules)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkPrimitiveParams(LSL_Types.LSLInteger link, LSL_Types.list rules)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetLinkPrimitiveParamsFast(LSL_Types.LSLInteger link, LSL_Types.list rules)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llSetPrimMediaParams(LSL_Types.LSLInteger face, LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetPrimURL(LSL_Types.LSLString url)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llSetRegionPos(LSL_Types.Vector3 position)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetRemoteScriptAccessPin(LSL_Types.LSLInteger pin)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetRot(LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetScale(LSL_Types.Vector3 size)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetScriptState(LSL_Types.LSLString name, LSL_Types.LSLInteger running)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetSitText(LSL_Types.LSLString text)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetSoundQueueing(LSL_Types.LSLInteger queue)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetSoundRadius(LSL_Types.LSLFloat radius)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetStatus(LSL_Types.LSLInteger status, LSL_Types.LSLInteger value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetText(LSL_Types.LSLString text, LSL_Types.Vector3 color, LSL_Types.LSLFloat alpha)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetTexture(LSL_Types.LSLString texture, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetTextureAnim(LSL_Types.LSLInteger mode, LSL_Types.LSLInteger face,
            LSL_Types.LSLInteger sizex, LSL_Types.LSLInteger sizey, LSL_Types.LSLFloat start, LSL_Types.LSLFloat length,
            LSL_Types.LSLFloat rate)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetTimerEvent(LSL_Types.LSLFloat sec)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetTorque(LSL_Types.Vector3 torque, LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetTouchText(LSL_Types.LSLString text)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetVehicleFlags(LSL_Types.LSLInteger flags)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetVehicleFloatParam(LSL_Types.LSLInteger param, LSL_Types.LSLFloat value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetVehicleRotationParam(LSL_Types.LSLInteger param, LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetVehicleType(LSL_Types.LSLInteger type)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetVehicleVectorParam(LSL_Types.LSLInteger param, LSL_Types.Vector3 vec)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSetVelocity(LSL_Types.Vector3 force, LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llSHA1String(LSL_Types.LSLString src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llShout(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llSin(LSL_Types.LSLFloat theta)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSitTarget(LSL_Types.Vector3 offset, LSL_Types.Quaternion rot)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSleep(LSL_Types.LSLFloat sec)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume, LSL_Types.LSLInteger queue,
            LSL_Types.LSLInteger loop)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llSoundPreload(LSL_Types.LSLString sound)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llSqrt(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStartAnimation(LSL_Types.LSLString anim)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStopAnimation(LSL_Types.LSLString anim)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStopHover()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStopLookAt()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStopMoveToTarget()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStopPointAt()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llStopSound()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llStringLength(LSL_Types.LSLString str)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llStringToBase64(LSL_Types.LSLString str)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llStringTrim(LSL_Types.LSLString src, LSL_Types.LSLInteger type)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llSubStringIndex(LSL_Types.LSLString source, LSL_Types.LSLString pattern)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTakeCamera(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTakeControls(LSL_Types.LSLInteger controls, LSL_Types.LSLInteger accept,
            LSL_Types.LSLInteger pass_on)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llTan(LSL_Types.LSLFloat theta)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llTarget(LSL_Types.Vector3 position, LSL_Types.LSLFloat range)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTargetOmega(LSL_Types.Vector3 axis, LSL_Types.LSLFloat spinrate, LSL_Types.LSLFloat gain)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTargetRemove(LSL_Types.LSLInteger handle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTeleportAgent(LSL_Types.key avatar, LSL_Types.LSLString landmark,
            LSL_Types.Vector3 position, LSL_Types.Vector3 look_at)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTeleportAgentGlobalCoords(LSL_Types.key agent, LSL_Types.Vector3 global_coordinates,
            LSL_Types.Vector3 region_coordinates, LSL_Types.Vector3 look_at)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTeleportAgentHome(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTextBox(LSL_Types.key avatar, LSL_Types.LSLString message, LSL_Types.LSLInteger channel)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llToLower(LSL_Types.LSLString src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llToUpper(LSL_Types.LSLString src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llTransferLindenDollars(LSL_Types.key destination, LSL_Types.LSLInteger amount)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTriggerSound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llTriggerSoundLimited(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume,
            LSL_Types.Vector3 top_north_east, LSL_Types.Vector3 bottom_south_west)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llUnescapeURL(LSL_Types.LSLString url)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llUnSit(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llUpdateCharacter(LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llVecDist(LSL_Types.Vector3 vec_a, LSL_Types.Vector3 vec_b)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llVecMag(LSL_Types.Vector3 vec)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llVecNorm(LSL_Types.Vector3 vec)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llVolumeDetect(LSL_Types.LSLInteger detect)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llWanderWithin(LSL_Types.Vector3 origin, LSL_Types.Vector3 dist, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llWater(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llWhisper(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llWind(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llXorBase64(LSL_Types.LSLString str1, LSL_Types.LSLString str2)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llXorBase64Strings(LSL_Types.LSLString str1, LSL_Types.LSLString str2)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llXorBase64StringsCorrect(LSL_Types.LSLString str1, LSL_Types.LSLString str2)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llAbs(LSL_Types.LSLInteger val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llAcos(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llAddToLandBanList(LSL_Types.key avatar, LSL_Types.LSLFloat hours)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llAddToLandPassList(LSL_Types.key avatar, LSL_Types.LSLFloat hours)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llAdjustSoundVolume(LSL_Types.LSLFloat volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llAllowInventoryDrop(LSL_Types.LSLInteger add)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llAngleBetween(LSL_Types.Quaternion a, LSL_Types.Quaternion b)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llApplyImpulse(LSL_Types.Vector3 momentum, LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llApplyRotationalImpulse(LSL_Types.Vector3 force, LSL_Types.LSLInteger local)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llAsin(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llAtan2(LSL_Types.LSLFloat y, LSL_Types.LSLFloat x)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llAttachToAvatar(LSL_Types.LSLInteger attach_point)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llAttachToAvatarTemp(LSL_Types.LSLInteger attach_point)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llAvatarOnLinkSitTarget(LSL_Types.LSLInteger link)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llAvatarOnSitTarget()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llAxes2Rot(LSL_Types.Vector3 fwd, LSL_Types.Vector3 left,
            LSL_Types.Vector3 up)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llAxisAngle2Rot(LSL_Types.Vector3 axis, LSL_Types.LSLFloat angle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llBase64ToInteger(LSL_Types.LSLString str)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llBase64ToString(LSL_Types.LSLString str)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llBreakAllLinks()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llBreakLink(LSL_Types.LSLInteger link)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llCSV2List(LSL_Types.LSLString src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llCastRay(LSL_Types.Vector3 start, LSL_Types.Vector3 end, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llCeil(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llClearCameraParams()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llClearLinkMedia(LSL_Types.LSLInteger link, LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llClearPrimMedia(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llCloseRemoteDataChannel(LSL_Types.key channel)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llCloud(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llCollisionFilter(LSL_Types.LSLString name, LSL_Types.key id, LSL_Types.LSLInteger accept)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llCollisionSound(LSL_Types.LSLString impact_sound, LSL_Types.LSLFloat impact_volume)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llCollisionSprite(LSL_Types.LSLString impact_sprite)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llCos(LSL_Types.LSLFloat theta)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llCreateCharacter(LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llCreateLink(LSL_Types.key target, LSL_Types.LSLInteger parent)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llDeleteCharacter()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llDeleteSubList(LSL_Types.list src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger end)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llDeleteSubString(LSL_Types.LSLString src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger end)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llDetachFromAvatar()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedGrab(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llDetectedGroup(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llDetectedKey(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llDetectedLinkNumber(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llDetectedName(LSL_Types.LSLInteger item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llDetectedOwner(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedPos(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llDetectedRot(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedTouchBinormal(LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llDetectedTouchFace(LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedTouchNormal(LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedTouchPos(LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedTouchST(LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedTouchUV(LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llDetectedType(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llDetectedVel(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llDialog(LSL_Types.key avatar, LSL_Types.LSLString message, LSL_Types.list buttons,
            LSL_Types.LSLInteger channel)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llDie()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llDumpList2String(LSL_Types.list src, LSL_Types.LSLString separator)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llEdgeOfWorld(LSL_Types.Vector3 pos, LSL_Types.Vector3 dir)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llEjectFromLand(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llEmail(LSL_Types.LSLString address, LSL_Types.LSLString subject,
            LSL_Types.LSLString message)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llEscapeURL(LSL_Types.LSLString url)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llEuler2Rot(LSL_Types.Vector3 v)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llEvade(LSL_Types.key target, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llExecCharacterCmd(LSL_Types.LSLInteger command, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llFabs(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llFleeFrom(LSL_Types.Vector3 position, LSL_Types.LSLFloat distance, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llFloor(LSL_Types.LSLFloat val)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llForceMouselook(LSL_Types.LSLInteger mouselook)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llFrand(LSL_Types.LSLFloat mag)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGenerateKey()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetAccel()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetAgentInfo(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetAgentLanguage(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetAgentList(LSL_Types.LSLInteger scope, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetAgentSize(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetAlpha(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetAndResetTime()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetAnimation(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetAnimationList(LSL_Types.key avatar)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetAnimationOverride(LSL_Types.LSLString anim_state)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetAttached()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetBoundingBox(LSL_Types.key object_key)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetCameraPos()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llGetCameraRot()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetCenterOfMass()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetClosestNavPoint(LSL_Types.Vector3 point, LSL_Types.list options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetColor(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetCreator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetDate()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetDisplayName(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetEnergy()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetEnv(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetForce()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetFreeMemory()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetFreeURLs()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetGMTclock()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetGeometricCenter()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetHTTPHeader(LSL_Types.key request_id, LSL_Types.LSLString header)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetInventoryCreator(LSL_Types.LSLString item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetInventoryKey(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetInventoryName(LSL_Types.LSLInteger type, LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetInventoryNumber(LSL_Types.LSLInteger type)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetInventoryPermMask(LSL_Types.LSLString item,
            LSL_Types.LSLInteger category)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetInventoryType(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetKey()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetLandOwnerAt(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetLinkKey(LSL_Types.LSLInteger link)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetLinkMedia(LSL_Types.LSLInteger link, LSL_Types.LSLInteger face,
            LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetLinkName(LSL_Types.LSLInteger link)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetLinkNumber()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetLinkNumberOfSides(LSL_Types.LSLInteger link)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetPrimitiveParams(LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetListEntryType(LSL_Types.list src, LSL_Types.LSLInteger index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetListLength(LSL_Types.list src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetLocalPos()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llGetLocalRot()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetMass()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetMassMKS()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetMaxScaleFactor()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetMemoryLimit()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetMinScaleFactor()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llGetNextEmail(LSL_Types.LSLString address, LSL_Types.LSLString subject)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetNotecardLine(LSL_Types.LSLString name, LSL_Types.LSLInteger line)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetNumberOfNotecardLines(LSL_Types.LSLString name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetNumberOfPrims()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetNumberOfSides()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetObjectDesc()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetObjectDetails(LSL_Types.key id, LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetObjectMass(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetObjectName()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetObjectPermMask(LSL_Types.LSLInteger category)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetObjectPrimCount(LSL_Types.key prim)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetOmega()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetOwner()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetOwnerKey(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetParcelDetails(LSL_Types.Vector3 pos, LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetParcelFlags(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetParcelMaxPrims(LSL_Types.Vector3 pos, LSL_Types.LSLInteger sim_wide)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetParcelMusicURL()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetParcelPrimCount(LSL_Types.Vector3 pos, LSL_Types.LSLInteger category,
            LSL_Types.LSLInteger sim_wide)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetParcelPrimOwners(LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetPermissions()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llGetPermissionsKey()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetPhysicsMaterial()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetPos()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetPrimMediaParams(LSL_Types.LSLInteger face, LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetLinkPrimitiveParams(LSL_Types.LSLInteger link, LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetRegionAgentCount()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetRegionCorner()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetRegionFPS()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetRegionFlags()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetRegionName()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetRegionTimeDilation()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetRootPosition()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llGetRootRotation()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Quaternion llGetRot()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetSPMaxMemory()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetScale()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetScriptName()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetScriptState(LSL_Types.LSLString script)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetSimStats(LSL_Types.LSLInteger stat_type)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetSimulatorHostname()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetStartParameter()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llGetStaticPath(LSL_Types.Vector3 start, LSL_Types.Vector3 end,
            LSL_Types.LSLFloat radius, LSL_Types.list parameters)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetStatus(LSL_Types.LSLInteger status)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetSubString(LSL_Types.LSLString src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger end)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetSunDirection()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetTexture(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetTextureOffset(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetTextureRot(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetTextureScale(LSL_Types.LSLInteger face)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetTime()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetTimeOfDay()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetTimestamp()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetTorque()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetUnixTime()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGetUsedMemory()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llGetUsername(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGetVel()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGetWallclock()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llGiveInventory(LSL_Types.key destination, LSL_Types.LSLString inventory)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llGiveInventoryList(LSL_Types.key target, LSL_Types.LSLString folder,
            LSL_Types.list inventory)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLInteger llGiveMoney(LSL_Types.key destination, LSL_Types.LSLInteger amount)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llGodLikeRezObject(LSL_Types.key inventory, LSL_Types.Vector3 pos)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLFloat llGround(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGroundContour(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGroundNormal(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llGroundRepel(LSL_Types.LSLFloat height, LSL_Types.LSLInteger water, LSL_Types.LSLFloat tau)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.Vector3 llGroundSlope(LSL_Types.Vector3 offset)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.key llHTTPRequest(LSL_Types.LSLString url, LSL_Types.list parameters,
            LSL_Types.LSLString body)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llHTTPResponse(LSL_Types.key request_id, LSL_Types.LSLInteger status,
            LSL_Types.LSLString body)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llInsertString(LSL_Types.LSLString dst, LSL_Types.LSLInteger pos,
            LSL_Types.LSLString src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override void llInstantMessage(LSL_Types.key user, LSL_Types.LSLString message)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llIntegerToBase64(LSL_Types.LSLInteger number)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.list llJson2List(LSL_Types.LSLString src)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llJsonGetValue(LSL_Types.LSLString json, LSL_Types.list specifiers)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llJsonSetValue(LSL_Types.LSLString json, LSL_Types.list specifiers,
            LSL_Types.LSLString value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llJsonValueType(LSL_Types.LSLString json, LSL_Types.list specifiers)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        public override LSL_Types.LSLString llKey2Name(LSL_Types.key id)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}