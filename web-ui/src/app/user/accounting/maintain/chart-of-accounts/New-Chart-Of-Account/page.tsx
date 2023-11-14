"use client";

import { ROUTES } from "@/lib/routes";
import { useEffect, useState } from "react";
import {
  Button,
  Checkbox,
  Form,
  Input,
  Modal,
  Radio,
  Select,
  Spin,
  Tooltip,
} from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import {
  MinusCircleOutlined,
  PlusOutlined,
  SyncOutlined,
} from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";
import NotificationService from "@/services/NotificationService";

export default observer(function Page() {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  // const {amount} = values && value;
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
  const [branchCode, setBranchCode] = useState("");
  const [allBranches, setAllBranched] = useState([]);
  const [AcctLevel, setAcctLevel] = useState([]);
  const [acctType, setacctType] = useState([]);
  const [theAcctCurrentLevel, setTheAcctCurrentLevel] = useState(undefined);
  const [allParentAcct, setAllParentAcct] = useState([]);

  const [ParentId, setParentId] = useState("");
  const [accountName, setAccountName] = useState<string>("");
  const [accountId, setAccountId] = useState<string>("");
  const [isDisabled, setIsDisabled] = useState(false);
  const [statementOfCashFlowData, setStatementOfCashFlowData] = useState([]);
  const [StatementOfIncome, setStatementOfIncome] = useState([]);
  const [StatementOfFinancialPostionData, setStatementOfFinancialPostionData] =
    useState([]);
  const [statementOfChangesInEquityData, setstatementOfChangesInEquityData] =
    useState([]);
  const initialValues = {
    transNo: "",

    accountType: "",
    branch: "",
    accountLevel: 0,
    bankAccount: "",
    pettyCashAccount: "",
    bankChargeAccount: "",
    incomeStateAnnual: 0,
    socfAnnual: 0,
    socieAnnual: 0,
    sofpAnnual: 0,
    previousYearCreditDebitAnnual: "",
    excludeInIFRSReporting: true,
    isInternal: true,
  };
  console.log(values);
  const navigateToChartOfAccounts = () => {
    router.push("/user/accounting/maintain/chart-of-accounts");
  };
  const onSuccessDepositClose = () => {
    setIsLoading(true);
    navigateToChartOfAccounts();
  };

  const showModal = async () => {
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(true);
    }
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const handleReset = () => {
    form.resetFields();
    setIsDisabled(false);
  };

  const fetchAllAccount = async () => {
    setIsLoading(true);
    const getAllBranchResponse = await depositStore.getAllBranch();
    const getAcctLevel = await depositStore.getAllAccountLevels();
    const getAcctType = await depositStore.getAllAccountTypes();
    const getStatementOfIncome = await depositStore.statementOfIncome();
    const getstatementOfCashFlow = await depositStore.statementOfCashFlow();
    const getStatementOfFinancialPostion =
      await depositStore.statementOfFinancialPostion();
    const getStatementOfChangesInEquity =
      await depositStore.statementOfChangesInEquity();
    if (
      getAllBranchResponse.kind === "ok" &&
      getAcctLevel.kind === "ok" &&
      getAcctType.kind === "ok" &&
      getStatementOfIncome.kind === "ok" &&
      getstatementOfCashFlow.kind === "ok" &&
      getStatementOfFinancialPostion.kind === "ok" &&
      getStatementOfChangesInEquity.kind === "ok"
    ) {
      setAllBranched(getAllBranchResponse.data.result);
      setAcctLevel(getAcctLevel.data.result);
      setacctType(getAcctType.data.result);
      setStatementOfIncome(getStatementOfIncome.data.result);
      setStatementOfCashFlowData(getstatementOfCashFlow.data.result);
      setStatementOfFinancialPostionData(
        getStatementOfFinancialPostion.data.result
      );
      setstatementOfChangesInEquityData(
        getStatementOfChangesInEquity.data.result
      );
      setIsLoading(false);
    } else {
      setAllBranched([]);
      setAcctLevel([]);
      setacctType([]);
      setStatementOfIncome([]);
      setStatementOfCashFlowData([]);
      setStatementOfFinancialPostionData([]);
      setstatementOfChangesInEquityData([]);
      setIsLoading(false);
    }
  };

  const fetchParent = async () => {
    if (branchCode.length > 0 && theAcctCurrentLevel !== undefined) {
      setIsLoading(true);
      const fetchParentresponse =
        await depositStore.getAllParentWithAccountLevelsAndBranchCode(
          branchCode,
          theAcctCurrentLevel
        );
      if (fetchParentresponse.kind === "ok") {
        setIsLoading(false);
        setAllParentAcct(fetchParentresponse.data.result);
      } else {
        setAllParentAcct([]);
        setIsLoading(false);
      }
    }
  };
  useEffect(() => {
    fetchParent();
  }, [branchCode, theAcctCurrentLevel]);
  // console.log(branchCode.length,theAcctCurrentLevel)
  useEffect(() => {
    fetchAllAccount();
  }, []);

  useEffect(() => {
    form.validateFields({ validateOnly: true });
  }, [values]);

  // submitting the form 
  const handleSubmit = async () => {
    console.log(values);
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.CreateMaintainChartOfAccount({
        ...values,
        parentId: ParentId,
        bankAccount: values.bankAccount === true ? "Y" : "N",
        pettyCashAccount: values.pettyCashAccount === true ? "Y" : "N",
        bankChargeAccount: values.bankChargeAccount === true ? "Y" : "N",
        accountId: accountId,
        accountName: accountName,
        previousYearCreditDebitAnnual: "",
        excludeInIFRSReporting: true,
      });
      if (response.kind === "ok") {
        setIsModalOpen(false);
        setIsModalOpen2(true);
      } else {
        setIsLoading(false);
      }
    }
  };
  const setAllFormValues = (accountId: string) => {
    if (!accountId || allParentAcct.length < 1) {
      return;
    }
    setParentId(accountId);
    allParentAcct
      .filter(
        (value: { accountId: string }, index) => value.accountId === accountId
      )
      .map((value: { accountLevel: number }, _) => {
        value.accountLevel += 1;
        form.setFieldsValue({ ...value });
        setIsDisabled(true);
      });
  };

  return (
    <Spin tip="Authenticating...." spinning={isLoading} className="text-[red] ">
      <main className="flex flex-col w-[95%] mx-auto">
        <div className="flex justify-end">
          <Button
            className="mb-5"
            type="primary"
            onClick={navigateToChartOfAccounts}
          >
            view saved
          </Button>
        </div>

        <Form
          form={form}
          initialValues={initialValues}
          className="bg-gray-50 p-8 rounded border"
          layout="vertical"
        >
          <div className="flex justify-end items-end  outline-none ">
            <Tooltip title="refresh" placement="bottom">
              <SyncOutlined
                onClick={handleReset}
                className="flex justify-end items-end cursor-pointer outline-none  "
              />
            </Tooltip>
          </div>
          <div className="flex gap-3">
            <Form.Item
              className="mb-3 flex-1"
              rules={[
                {
                  required: true,
                  message: "Enter Description",
                },
              ]}
              label="Account ID "
            >
              <Input
                placeholder="01"
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setAccountId(e.target.value)
                }
              />
            </Form.Item>
            <Form.Item
              className="mb-3 flex-1"
              rules={[
                {
                  required: true,
                  message: "Enter Description",
                },
              ]}
              label="Description "
            >
              <Input
                placeholder="Ayodeji "
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setAccountName(e.target.value)
                }
              />
            </Form.Item>
          </div>
          <div className="flex gap-3">
            <Form.Item
              name={"branch"}
              className="mb-3 flex-1"
              label="Branch  "
              rules={[{ required: true }]}
            >
              <Select
                placeholder="Branch "
                className="text-gray-600"
                onSelect={setBranchCode}
                disabled={isDisabled}
              >
                {allBranches.map(({ transNo, nameWithCode }, index) => (
                  <Select.Option value={transNo} key={index}>
                    {nameWithCode}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"accountLevel"}
              className="mb-3 flex-1"
              label="Account level  "
              rules={[{ required: true }]}
            >
              <Select
                placeholder="Choose Account level "
                className="text-gray-600"
                onSelect={setTheAcctCurrentLevel}
                disabled={isDisabled}
              >
                {AcctLevel.map(({ acctLevel }, index) => (
                  <Select.Option value={acctLevel} key={index}>
                    {acctLevel}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"parentId"}
              className="mb-3 flex-1"
              label="parent Account "
              // rules={[{ required: true }]}
            >
              <Select
                placeholder="parent account"
                className="text-gray-600"
                onSelect={(value) => setAllFormValues(value)}
                // disabled={isDisabled}
              >
                {allParentAcct.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="flex gap-3">
            <Form.Item
              name={"accountType"}
              label="Account type  "
              className="mb-3 flex-1"
              rules={[{ required: true }]}
            >
              <Select
                placeholder="Choose Account Type "
                className="text-gray-600"
                // onSelect={setacctType}
                disabled={isDisabled}
              >
                {acctType.map(({ code, description }, index) => (
                  <Select.Option value={code} key={index}>
                    {description}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <Form.Item
            name={"isInternal"}
            className="mb-3 flex-1"
            rules={[{ required: true }]}
          >
            <Radio.Group disabled={isDisabled}>
              <Radio value={true}>Internal</Radio>
              <Radio value={false}>Control</Radio>
            </Radio.Group>
          </Form.Item>
          <Form.Item
            name="bankAccount"
            className="mb-3 flex-1"
            valuePropName="checked"
            // rules={[{ required: true }]}
          >
            <Checkbox disabled={isDisabled}>Bank Account</Checkbox>
          </Form.Item>
          <Form.Item
            name="pettyCashAccount"
            className="mb-3 flex-1"
            valuePropName="checked"
            // rules={[{ required: true }]}
          >
            <Checkbox disabled={isDisabled}>petty cash</Checkbox>
          </Form.Item>{" "}
          <Form.Item
            name="bankChargeAccount"
            className="mb-3 flex-1"
            valuePropName="checked"
            // rules={[{ required: true }]}
          >
            <Checkbox disabled={isDisabled}>Bank charges</Checkbox>
          </Form.Item>
          <p className="font-[600] leading-relaxed py-5">IFRS Reporting</p>
          <div className="flex gap-3">
            <Form.Item
              name={"socfAnnual"}
              className="mb-3 flex-1"
              label="Statement Of Cash Flow "
              rules={[{ required: true }]}
            >
              <Select
                placeholder="Statement Of Cash Flow"
                className="text-gray-600"
                disabled={isDisabled}
              >
                {statementOfCashFlowData.map(({ transNo, itemName }, index) => (
                  <Select.Option value={transNo} key={index}>
                    {itemName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"socieAnnual"}
              className="mb-3 flex-1"
              label="Statement Of Changes In Equity "
              // rules={[{ required: true }]}
            >
              <Select
                placeholder="Statement Of Changes In Equity"
                className="text-gray-600"
                disabled={isDisabled}
              >
                {statementOfChangesInEquityData.map(
                  ({ transNo, itemName }, index) => (
                    <Select.Option value={transNo} key={index}>
                      {itemName}
                    </Select.Option>
                  )
                )}
              </Select>
            </Form.Item>
          </div>
          <div className="flex gap-3">
            <Form.Item
              name={"sofpAnnual"}
              className="mb-3 flex-1"
              label="Statement Of Financial Postion "
              rules={[{ required: true }]}
            >
              <Select
                placeholder="Statement Of Financial Postion"
                className="text-gray-600"
                disabled={isDisabled}
              >
                {StatementOfFinancialPostionData.map(
                  ({ transNo, itemName }, index) => (
                    <Select.Option value={transNo} key={index}>
                      {itemName}
                    </Select.Option>
                  )
                )}
              </Select>
            </Form.Item>
            <Form.Item
              name={"incomeStateAnnual"}
              className="mb-3 flex-1"
              label="Statement Of Income "
              // rules={[{ required: true }]}
            >
              <Select
                placeholder="Statement Of Income"
                className="text-gray-600"
                disabled={isDisabled}
              >
                {StatementOfIncome.map(({ transNo, itemName }, index) => (
                  <Select.Option value={transNo} key={index}>
                    {itemName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="flex justify-end mt-10">
            <Button type="primary" htmlType="submit" onClick={showModal}>
              Save
            </Button>
          </div>
        </Form>

        <Modal
          open={isModalOpen}
          centered={true}
          okText="save"
          cancelText="Make Changes"
          onOk={handleSubmit}
          onCancel={handleCancel}
          footer={[
            <div key={2} className=" w-[80%] mx-auto flex justify-center items-center">
              <Button
                key="back"
                htmlType="submit"
                className="w-full bg-[#EDF2F7] text-[#194BFB]"
                onClick={handleCancel}
              >
                Make Changes
              </Button>
              <Button
                key="submit"
                type="primary"
                className="w-full"
                onClick={handleSubmit}
              >
                Save
              </Button>
            </div>,
          ]}
        >
          <div className="w-[80%] mx-auto justify-center items-center text-center mt-8 gap-5">
            <Image src={images.warningModalImage} alt="warning_Modal_Image" />
            <p className="text-[20px] leading-[26px] font-bold">
              Are you sure you want to save?
            </p>
            <p className="text-[14px] leading-[21px] ">
              Make sure you have inserted all the entries correctly or go back
              to make changes.
            </p>
          </div>
        </Modal>
        {/* modal for sucessfully saved deposit */}
        <Modal
          open={isModalOpen2}
          onCancel={() => onSuccessDepositClose()}
          centered={true}
          footer={[
            <div
              key={4}
              className=" w-full flex justify-center items-center capitalize"
            >
              <p className="text-[20px] font-bold leading-[26px] text-center">
                deposit saved sucessfully
              </p>
            </div>,
          ]}
        >
          <div className="w-[80%] mx-auto justify-center items-center text-center mt-8 gap-5">
            <Image src={images.successModalImage} alt="warning_Modal_Image" />
          </div>
        </Modal>
      </main>
    </Spin>
  );
});
