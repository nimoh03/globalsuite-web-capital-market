"use client";

import { CustomCard } from "@/app/components/customCard";
import { Button, Checkbox, DatePicker, Form, Radio, Select } from "antd";
import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [form] = Form.useForm();
  return (
    <CustomCard
      cardClassname={"mx-auto"}
      headerContent={<p>Creditors And Debtors</p>}
    >
      <Form form={form} className="bg-gray-50 py-4 px-8 " layout="vertical">
        <div className="flex gap-3 ">
          <Form.Item
            name={"startDate"}
            className="flex-1 mb-1"
            label="From Date"
            rules={[{ required: true }]}
          >
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>

          <Form.Item
            name={"endDate"}
            className="flex-1 mb-1"
            label="Balance As At"
            rules={[{ required: true }]}
          >
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>
        </div>
        <div className="flex gap-3 ">
          <Form.Item
            name={"productAccount"}
            label={"Parent Account"}
            className="flex-1 "
            rules={[{ required: true }]}
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>

          <Form.Item
            name={"customerAccount"}
            label={"Customer Account"}
            className="flex-1 "
            rules={[{ required: true }]}
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>
        </div>

        <div>
          <Form.Item
            name={"accountSelection"}
            rules={[{ required: true }]}
            initialValue={1}
            className="mb-0"
          >
            <Radio.Group>
              <Radio value={1}>All (Exc.0)</Radio>
              <Radio value={2}>Creditors Only</Radio>
              <Radio value={3}>Debtors Only</Radio>
              <Radio value={4}>By Balance Amount</Radio>
              <Radio value={5}>All (Inc.0)</Radio>
            </Radio.Group>
          </Form.Item>
        </div>

        <div className="flex items-center">
          <Form.Item
            name={"accountSelection4"}
            rules={[{ required: true }]}
            className="mb-0 flex-1"
          >
            <Checkbox>Exclude Account With Zero Balances</Checkbox>
          </Form.Item>

          <div className="flex-1">
            <p className="pl-5 mb-1">Balances Amount</p>
            <div className="flex gap-3">
              <Form.Item
                name={"balanceFromDate"}
                rules={[{ required: true }]}
                className="mb-0 w-6/12"
              >
                <DatePicker className="w-full" format="DD/MM/YYYY" />
              </Form.Item>

              <Form.Item
                name={"balanceToDate"}
                rules={[{ required: true }]}
                className="mb-0 w-6/12"
              >
                <DatePicker className="w-full" format="DD/MM/YYYY" />
              </Form.Item>
            </div>
          </div>
        </div>

        <div>
          <Form.Item
            name={"accountSelection333"}
            rules={[{ required: true }]}
            initialValue={1}
            className="mb-0"
          >
            <Radio.Group>
              <Radio value={1}>Closing Balance</Radio>
              <Radio value={2}>Movement</Radio>
              <Radio value={3}>No Opening Balance</Radio>
              <Radio value={4}>With Opening Balance</Radio>
            </Radio.Group>
          </Form.Item>
        </div>
        <div className="flex flex-wrap">
          <Form.Item
            name={"accountSelection1"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>
              Exclude Accounts Tagged Exclude In A/C Deactivation
            </Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection2"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Print With Email Address</Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection3"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Exclude Staff Loan Product Ledger Balances</Checkbox>
          </Form.Item>

          <Form.Item
            name={"accountSelection4"}
            rules={[{ required: true }]}
            className="mb-0 w-6/12"
          >
            <Checkbox>Group By Customer (Concluded)</Checkbox>
          </Form.Item>
        </div>
        <div>
          <Form.Item
            label="Branch"
            name={"branch"}
            rules={[{ required: true }]}
            className="mb-0 flex-1 max-w-[180px]"
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>
        </div>
        <div className="flex justify-end">
          <Button className="bg-blue-200 text-white px-4 py-2 rounded">
            View
          </Button>
        </div>
      </Form>
    </CustomCard>
  );
});
