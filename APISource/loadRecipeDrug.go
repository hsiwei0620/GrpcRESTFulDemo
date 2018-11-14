package APISource

import (
	msmqCon "path/to/project/connectionConfig"
	msmqSend "path/to/project/protos/MqGrpcProject"
	API "path/to/project/protos/callAPIService"
	"strconv"
)

//LoadDrug Start
func LoadDrug(req *API.Load_Recipe_Drug_Request, mesServer string) (API.Load_Recipe_Drug_Reply, error) {

	//initial variable
	var reAry API.Load_Recipe_Drug_Reply // 回傳Reply內容
	reMrtlCnt := 0                       // 回傳mtrlCnt的筆數
	reOrderCnt := 0                      // 回傳工單的筆數
	aplprdbmOary1 := new(API.AplprdbmA)  // aplprdbm.oary1

	conn, ctx, myCloseClient, err := msmqCon.Connection()
	if err != nil {
		return reAry, err
	}
	defer myCloseClient()
	client := msmqSend.NewMqGrpcsClient(conn)

	// Send APLRSVPR Start : 取得配藥工單
	APLRSVPR, err := client.APLRSVPR_Send(ctx, &msmqSend.APLRSVPR_Request{
		Serverip:      mesServer,
		Bayid:         req.GetBayid(),
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
	reAry.Lotarycnt = APLRSVPR.GetLotarycnt() // return count
	inLotCnt, _ := strconv.Atoi(APLRSVPR.GetLotarycnt())
	if inLotCnt == 0 {
		reAry.Rtncode = "2"
		reAry.Rtnmesg = "找到不到配藥(APLRSVPRo.lot_ary_cnt=0)"
		return reAry, nil
	}

	// 抓出每包藥包內容
	for i := 0; i < inLotCnt; i++ {
		apmeqrsvOary := new(API.ApmeqrsvA) // apmeqrsv.oary
		aplrsvprOary := new(API.AplrsvprA) // aplrsvpr.oary

		// return aplrsvpr
		aplrsvprOary.Lotid = APLRSVPR.GetOary()[i].GetLotid()
		aplrsvprOary.Lotstat = APLRSVPR.GetOary()[i].GetLotstat()
		aplrsvprOary.Nxopeno = APLRSVPR.GetOary()[i].GetNxopeno()
		aplrsvprOary.Mtrlproductid = APLRSVPR.GetOary()[i].GetMtrlproductid()
		aplrsvprOary.Preptype = APLRSVPR.GetOary()[i].GetPreptype()
		aplrsvprOary.Prepstat = APLRSVPR.GetOary()[i].GetPrepstat()
		aplrsvprOary.Prepdate = APLRSVPR.GetOary()[i].GetPrepdate()
		aplrsvprOary.Prepseqno = APLRSVPR.GetOary()[i].GetPrepseqno()
		aplrsvprOary.Resvdate = APLRSVPR.GetOary()[i].GetResvdate()
		aplrsvprOary.Resvshiftseq = APLRSVPR.GetOary()[i].GetResvshiftseq()
		aplrsvprOary.Claimdate = APLRSVPR.GetOary()[i].GetClaimdate()
		aplrsvprOary.Claimtime = APLRSVPR.GetOary()[i].GetClaimtime()
		aplrsvprOary.Claimuser = APLRSVPR.GetOary()[i].GetClaimuser()
		aplrsvprOary.Nxopeid = APLRSVPR.GetOary()[i].GetNxopeid()
		aplrsvprOary.Nxopever = APLRSVPR.GetOary()[i].GetNxopever()
		aplrsvprOary.Spltid = APLRSVPR.GetOary()[i].GetSpltid()
		aplrsvprOary.Resveqptid = APLRSVPR.GetOary()[i].GetResveqptid()
		aplrsvprOary.Shtcnt = APLRSVPR.GetOary()[i].GetShtcnt()
		aplrsvprOary.Readyshtcnt = APLRSVPR.GetOary()[i].GetReadyshtcnt()
		aplrsvprOary.Productid = APLRSVPR.GetOary()[i].GetProductid()
		aplrsvprOary.Eccode = APLRSVPR.GetOary()[i].GetEccode()
		aplrsvprOary.Mainrouteid = APLRSVPR.GetOary()[i].GetMainrouteid()
		aplrsvprOary.Mainroutever = APLRSVPR.GetOary()[i].GetMainroutever()
		aplrsvprOary.Cropeno = APLRSVPR.GetOary()[i].GetCropeno()
		aplrsvprOary.Lineid = APLRSVPR.GetOary()[i].GetLineid()
		reAry.Oary = append(reAry.Oary, aplrsvprOary) // add to reply array

		// Send APMEQRSV
		APMEQRSV, err := client.APMEQRSV_Send(ctx, &msmqSend.APMEQRSV_Request{
			Serverip:    mesServer,
			Actiontype:  "Q",
			Resveqptid:  APLRSVPR.Oary[i].GetResveqptid(),
			Lotid:       APLRSVPR.Oary[i].GetLotid(),
			Onlyresvflg: "Y"})
		if err != nil {
			return reAry, err
		}

		if gtLotAryCnt, _ := strconv.Atoi(APMEQRSV.GetLotarycnt()); gtLotAryCnt == 0 {
			continue
		} else {
			apmeqrsvOary.Lotid = APMEQRSV.GetOary()[0].GetLotid()
			apmeqrsvOary.Nxopeno = APMEQRSV.GetOary()[0].GetNxopeno()
			apmeqrsvOary.Nxopeid = APMEQRSV.GetOary()[0].GetNxopeid()
			apmeqrsvOary.Nxopever = APMEQRSV.GetOary()[0].GetNxopever()
			apmeqrsvOary.Resveqptid = APMEQRSV.GetOary()[0].GetResveqptid()
			apmeqrsvOary.Productid = APMEQRSV.GetOary()[0].GetProductid()
			apmeqrsvOary.Eccode = APMEQRSV.GetOary()[0].GetEccode()
			reAry.Oary3 = append(reAry.Oary3, apmeqrsvOary) // add to reply array
			reOrderCnt++
		}

		if apmeqrsvOary != nil {
			// Send APLPRDBM
			APLPRDBM, err := client.APLPRDBM_Send(ctx, &msmqSend.APLPRDBM_Request{
				Serverip:   mesServer,
				Productid:  apmeqrsvOary.Productid,
				Eccode:     apmeqrsvOary.Eccode,
				Opeid:      apmeqrsvOary.Nxopeid,
				Opever:     apmeqrsvOary.Nxopever,
				Resveqptid: APLRSVPR.Oary[i].GetResveqptid()})
			if err != nil {
				return reAry, err
			}

			reAry.Bomcnt = APLPRDBM.GetBomcnt()
			if BomCnt, _ := strconv.Atoi(APLPRDBM.GetBomcnt()); BomCnt > 0 {
				foundFlag = false
				for j := 0; j < BomCnt; j++ {
					if APLPRDBM.GetOary1()[j].GetBomid() == apmeqrsvOary.Nxopeid {
						aplprdbmOary1.OpeId = APLPRDBM.GetOary1()[j].GetOpeid()
						aplprdbmOary1.OpeVer = APLPRDBM.GetOary1()[j].GetOpever()
						aplprdbmOary1.BomId = APLPRDBM.GetOary1()[j].GetBomid()
						foundFlag = true
						reAry.Oary1 = append(reAry.Oary1, aplprdbmOary1)
						break
					}
				}
				if foundFlag == false {
					reAry.Rtncode = "4"
					reAry.Rtnmesg = "APLPRDBMo 找不到配藥(PLPRDBMo.bom_cnt=0),product_id: " +
						apmeqrsvOary.Productid + "nx_ope_id: " + apmeqrsvOary.Nxopeid
					return reAry, nil
				}
			} else {
				reAry.Rtncode = "4"
				reAry.Rtnmesg = "APLPRDBMo 找不到配藥(PLPRDBMo.bom_cnt=0),product_id: " +
					apmeqrsvOary.Productid + "nx_ope_id: " + apmeqrsvOary.Nxopeid
				return reAry, nil
			}

			// 配藥的明細(找出配藥)
			if mtrlCnt, _ := strconv.Atoi(APLPRDBM.GetOary1()[1].GetMtrlcnt()); mtrlCnt > 0 {
				for j := 0; j < mtrlCnt; j++ {
					if APLPRDBM.GetOary1()[1].GetOary2()[j].GetMtrlproductid() == APLRSVPR.Oary[i].GetMtrlproductid() {
						bom := new(API.Bom) // aplprdbm.oary1.oary2
						bom.Mtrlproductid = APLPRDBM.GetOary1()[1].GetOary2()[j].GetMtrlproductid()
						bom.Mtrlproductdsc = APLPRDBM.GetOary1()[1].GetOary2()[j].GetMtrlproductdsc()
						bom.Mtrlcate = APLPRDBM.GetOary1()[1].GetOary2()[j].GetMtrlcate()
						bom.Planqty = APLPRDBM.GetOary1()[1].GetOary2()[j].GetPlanqty()
						bom.Mtrlunit = APLPRDBM.GetOary1()[1].GetOary2()[j].GetMtrlunit()
						bom.Spcfymtrllotid = APLPRDBM.GetOary1()[1].GetOary2()[j].GetSpcfymtrllotid()
						bom.Ext1 = APLPRDBM.GetOary1()[1].GetOary2()[j].GetExt_1()
						bom.Ext2 = APLPRDBM.GetOary1()[1].GetOary2()[j].GetExt_2()
						bom.Ext3 = APLPRDBM.GetOary1()[1].GetOary2()[j].GetExt_3()
						bom.Ext4 = APLPRDBM.GetOary1()[1].GetOary2()[j].GetExt_4()
						bom.Ext5 = APLPRDBM.GetOary1()[1].GetOary2()[j].GetExt_5()
						bom.Parentid = APLPRDBM.GetOary1()[1].GetOary2()[j].GetParentid()
						reAry.Oary2 = append(reAry.Oary2, bom)

						//child
						index := 0
						for x := 0; x < mtrlCnt; x++ {
							if APLPRDBM.GetOary1()[1].GetOary2()[x].GetParentid() == APLRSVPR.Oary[i].GetMtrlproductid() {
								bom2 := new(API.Bom2)
								bom2.Mtrlproductid = APLPRDBM.GetOary1()[1].GetOary2()[x].GetMtrlproductid()
								bom2.Mtrlproductdsc = APLPRDBM.GetOary1()[1].GetOary2()[x].GetMtrlproductdsc()
								bom2.Mtrlcate = APLPRDBM.GetOary1()[1].GetOary2()[x].GetMtrlcate()
								bom2.Planqty = APLPRDBM.GetOary1()[1].GetOary2()[x].GetPlanqty()
								bom2.Mtrlunit = APLPRDBM.GetOary1()[1].GetOary2()[x].GetMtrlunit()
								bom2.Spcfymtrllotid = APLPRDBM.GetOary1()[1].GetOary2()[x].GetSpcfymtrllotid()
								bom2.Ext1 = APLPRDBM.GetOary1()[1].GetOary2()[x].GetExt_1()
								bom2.Ext2 = APLPRDBM.GetOary1()[1].GetOary2()[x].GetExt_2()
								bom2.Ext3 = APLPRDBM.GetOary1()[1].GetOary2()[x].GetExt_3()
								bom2.Ext4 = APLPRDBM.GetOary1()[1].GetOary2()[x].GetExt_4()
								bom2.Ext5 = APLPRDBM.GetOary1()[1].GetOary2()[x].GetExt_5()
								bom2.Parentid = APLPRDBM.GetOary1()[1].GetOary2()[x].GetParentid()
								reAry.Oary2[reMrtlCnt].Oary4 = append(reAry.Oary2[reMrtlCnt].Oary4, bom2)
								index++
							}
						}
						reAry.Oary2[reMrtlCnt].Oary4Cnt = strconv.Itoa(index)
						reMrtlCnt++
					}
				}

			} else {
				reAry.Rtncode = "8"
				reAry.Rtnmesg = "APLPRDBMo 找不到配藥(PLPRDBMo.bom_cnt=0),product_id: " +
					apmeqrsvOary.Productid + "nx_ope_id: " + apmeqrsvOary.Nxopeid
				return reAry, nil
			}
			reAry.Mtrlcnt = strconv.Itoa(reMrtlCnt)
		}
	}
	reAry.Ordercnt = strconv.Itoa(reOrderCnt)
	return reAry, nil
}
