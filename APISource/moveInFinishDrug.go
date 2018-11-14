package APISource

import (
	TX "path/to/project/protos/MqGrpcProject"
	msmqSend "path/to/project/protos/MqGrpcProject"
	API "path/to/project/protos/callAPIService"
	msmqCon "path/to/project/connectionConfig"
	"strconv"
)

func MoveInFinishDrug(req *API.Move_In_Finish_Drug_Request, mesServer string) (API.Move_In_Finish_Drug_Reply, error) {

	//initial variable
	var reAry API.Move_In_Finish_Drug_Reply // 回傳Reply內容
	reDruginfocnt := 0                      //回傳執行比數
	var sendIaryTemp []*TX.APCMTLSTiA       // 送 APCMTLST iary1 結構用

	aplrsvprOary := new(TX.APLRSVPRoA)   // 暫存APLRSVPR oary
	apmeqrsvOary := new(TX.APMEQRSVoA1)  // 暫存APMEQRSV oary
	aplprdbmOary1 := new(TX.APLPRDBMoA1) // 暫存APLPRDBM oary1
	druginfo := new(API.Druginfo)        // 暫存APLPRDBM oary1.oary2
	// apcmtlstI := new(TX.APCMTLST_Request) // 暫存APCMTLSTi
	apcmtlstIary1 := new(TX.APCMTLSTiA) // 暫存APCMTLST iary1

	// msmq connection
	conn, ctx, myCloseClient, err := msmqCon.Connection()
	if err != nil {
		return reAry, err
	}
	defer myCloseClient()
	client := msmqSend.NewMqGrpcsClient(conn)

	// Send APLRSVPR
	APLRSVPR, err := client.APLRSVPR_Send(ctx, &msmqSend.APLRSVPR_Request{
		Serverip:      mesServer,
		Lotid:         req.GetLotid(),
		Bayid:         req.GetBayid(),
		Resveqptid:    req.GetResveqptid(),
		Prepstartdate: req.GetPrepstartdate(),
		Prependdate:   req.GetPrependdate(),
		Needorderflg:  req.GetNeedorderflag(),
		Preptype:      req.GetPreptype()})
	if err != nil {
		return reAry, err
	}
	if rtncode, _ := strconv.Atoi(APLRSVPR.GetRtncode()); rtncode > 0 {
		reAry.Rtncode = APLRSVPR.GetRtncode()
		reAry.Rtnmesg = "APLRSVPR:" + APLRSVPR.GetRtnmesg()
		return reAry, nil
	}

	if inLotCnt, _ := strconv.Atoi(APLRSVPR.GetLotarycnt()); inLotCnt > 0 {
		foundFlag = false
		for i := 0; i < inLotCnt; i++ {
			if APLRSVPR.GetOary()[i].GetLotid() == req.GetLotid() &&
				APLRSVPR.GetOary()[i].GetMtrlproductid() == req.GetMtrlproductid() {
				aplrsvprOary = APLRSVPR.GetOary()[i]
				foundFlag = true
				break
			}
		}

		if foundFlag == false {
			reAry.Rtncode = "1"
			reAry.Rtnmesg = "APLRSVPR 藥包(找不到符合的), 工單: " + req.GetLotid() +
				"\n機台: " + req.GetResveqptid() + "\n藥包: " + req.GetMtrlproductid()
			return reAry, nil
		}
	} else {
		reAry.Rtncode = "1"
		reAry.Rtnmesg = "找到不到配藥(APLRSVPRo.lot_ary_cnt=0) 工單: " + req.GetLotid() +
			"\n機台: " + req.GetResveqptid() + "\n藥包: " + req.GetMtrlproductid()
		return reAry, nil
	}

	// Send APMEQRSV
	APMEQRSV, err := client.APMEQRSV_Send(ctx, &msmqSend.APMEQRSV_Request{
		Serverip:    mesServer,
		Actiontype:  "Q",
		Resveqptid:  aplrsvprOary.GetResveqptid(),
		Lotid:       aplrsvprOary.GetLotid(),
		Onlyresvflg: "Y"})
	if err != nil {
		return reAry, err
	}

	if gtLotAryCnt, _ := strconv.Atoi(APMEQRSV.GetLotarycnt()); gtLotAryCnt == 0 {
		reAry.Rtncode = "1"
		reAry.Rtnmesg = "APMEQRSVo 找不到工單(APMEQRSVo.lot_ary_cnt=0),工單: " +
			aplrsvprOary.GetLotid() + "\n機台: " + aplrsvprOary.GetResveqptid()
		return reAry, nil
	} else {
		apmeqrsvOary = APMEQRSV.GetOary()[0] //取得第一筆工單資訊
	}

	// Send APLPRDBM
	APLPRDBM, err := client.APLPRDBM_Send(ctx, &msmqSend.APLPRDBM_Request{
		Serverip:  mesServer,
		Productid: apmeqrsvOary.Productid,
		Eccode:    apmeqrsvOary.Eccode,
		Opeid:     apmeqrsvOary.Nxopeid,
		Opever:    apmeqrsvOary.Nxopever})
	if err != nil {
		return reAry, err
	}

	if BomCnt, _ := strconv.Atoi(APLPRDBM.GetBomcnt()); BomCnt > 0 {
		foundFlag = false
		for i := 0; i < BomCnt; i++ {
			if APLPRDBM.GetOary1()[i].GetBomid() == apmeqrsvOary.Nxopeid {
				aplprdbmOary1 = APLPRDBM.GetOary1()[i]
				foundFlag = true
				break
			}
		}
		if foundFlag == false {
			reAry.Rtncode = "4"
			reAry.Rtnmesg = "APLPRDBMo 找不到配藥(APLPRDBMo.bom_cnt>0),product_id: " +
				apmeqrsvOary.Productid + "\nnx_ope_id: " + apmeqrsvOary.Nxopeid
			return reAry, nil
		}
	} else {
		reAry.Rtncode = "4"
		reAry.Rtnmesg = "APLPRDBMo 找不到配藥(APLPRDBMo.bom_cnt=0),product_id: " +
			apmeqrsvOary.Productid + "\nnx_ope_id: " + apmeqrsvOary.Nxopeid
		return reAry, nil
	}

	//配藥的明細(找出配藥)
	if mtrlCnt, _ := strconv.Atoi(aplprdbmOary1.Mtrlcnt); mtrlCnt > 0 {
		foundFlag = false
		for i := 0; i < mtrlCnt; i++ {
			if aplprdbmOary1.GetOary2()[i].GetMtrlproductid() == req.GetMtrlproductid() {
				druginfo.Mtrlproductid = aplprdbmOary1.GetOary2()[i].GetMtrlproductid()
				druginfo.Mtrlproductdsc = aplprdbmOary1.GetOary2()[i].GetMtrlproductdsc()
				druginfo.Planqty = aplprdbmOary1.GetOary2()[i].GetPlanqty()
				druginfo.Mtrlunit = aplprdbmOary1.GetOary2()[i].GetMtrlunit()
				druginfo.Ext_1 = aplprdbmOary1.GetOary2()[i].GetExt_1()
				druginfo.Ext_2 = aplprdbmOary1.GetOary2()[i].GetExt_2()
				druginfo.Ext_3 = aplprdbmOary1.GetOary2()[i].GetExt_3()
				druginfo.Ext_4 = aplprdbmOary1.GetOary2()[i].GetExt_4()
				druginfo.Ext_5 = aplprdbmOary1.GetOary2()[i].GetExt_5()
				druginfo.Parentid = aplprdbmOary1.GetOary2()[i].GetParentid()
				reDruginfocnt++
				reAry.Oary = append(reAry.Oary, druginfo) // add to reply array
				foundFlag = true
				break
			}
		}
		if foundFlag == false {
			reAry.Rtncode = "8"
			reAry.Rtnmesg = "APLPRDBMo 找不到配藥明細(APLPRDBMo_a1.mtrl_cnt>0),product_id: " +
				apmeqrsvOary.Productid + "\nnx_ope_id: " + apmeqrsvOary.Nxopeid
			return reAry, nil
		}
		reAry.Druginfocnt = strconv.Itoa(reDruginfocnt)
	} else {
		reAry.Rtncode = "8"
		reAry.Rtnmesg = "APLPRDBMo 找不到配藥明細(APLPRDBMo_a1.mtrl_cnt=0),product_id: " +
			apmeqrsvOary.Productid + "\nnx_ope_id: " + apmeqrsvOary.Nxopeid
		return reAry, nil
	}

	// Consolidation APCMTLST Input Data
	apcmtlstIary1.Mtrlproductid = req.GetMtrlproductid()
	apcmtlstIary1.Mtrllotid = req.GetMtrllotid()
	apcmtlstIary1.Mtrlstat = "AVAL"
	apcmtlstIary1.Mtrlqty = req.GetMtrlqty()
	apcmtlstIary1.Pershtqty = req.GetPershtqty()
	apcmtlstIary1.Shtcnt = req.GetShtcnt()
	apcmtlstIary1.Comment = "PREP:" + aplrsvprOary.Preptype + ":" + aplrsvprOary.Lotid + ":" +
		aplrsvprOary.Nxopeno + ":" + aplrsvprOary.Mtrlproductid + ":" + req.GetMtrlqty()
	apcmtlstIary1.Tareqptid = aplrsvprOary.Resveqptid
	apcmtlstIary1.Expiredate = req.GetExpireday()
	apcmtlstIary1.Flag = "P"

	apcmtlstIary1.Iary2Cnt = req.GetUsemtrlcnt()
	usedMtrlCnt, _ := strconv.Atoi(req.GetUsemtrlcnt())
	for i := 0; i < usedMtrlCnt; i++ {
		apcmtlstIary2 := new(TX.APCMTLSTiA2) // 暫存APCMTLST iary1.iary2
		apcmtlstIary2.Usemtrlproductid = req.GetIary2()[i].GetUsemtrlproductid()
		apcmtlstIary2.Usemtrllotid = req.GetIary2()[i].GetUsemtrllotid()
		apcmtlstIary2.Useqty = req.GetIary2()[i].GetUseqty()
		apcmtlstIary2.Useseqno = req.GetIary2()[i].GetUseseqno()
		apcmtlstIary2.Lotid = req.GetIary2()[i].GetLotid()
		apcmtlstIary1.Iary2 = append(apcmtlstIary1.Iary2, apcmtlstIary2)
	}

	// Send APCMTLST
	sendIaryTemp = append(sendIaryTemp, apcmtlstIary1)

	APCMTLST, err := client.APCMTLST_Send(ctx, &msmqSend.APCMTLST_Request{
		Serverip:   mesServer,
		Clmmtsttyp: "A",
		Userid:     req.GetEmpno(),
		Bomid:      aplprdbmOary1.GetBomid(),
		Mtrlcnt:    "1",
		Iary:       sendIaryTemp})
	if err != nil {
		return reAry, err
	}
	if rtncode, _ := strconv.Atoi(APCMTLST.GetRtncode()); rtncode > 0 {
		reAry.Rtncode = APCMTLST.GetRtncode()
		reAry.Rtnmesg = "APCMTLST:" + APCMTLST.GetRtnmesg()
		return reAry, nil
	}

	return reAry, nil
}
