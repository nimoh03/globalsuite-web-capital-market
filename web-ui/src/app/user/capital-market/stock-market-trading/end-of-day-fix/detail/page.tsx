'use client'
import { Descriptions , Progress , Form , Checkbox , Button } from "antd"
import { TagOutlined } from "@ant-design/icons"
const EndOfTheDay = () => {

    const items = [
        {
          label: (
            <div className="flex items-center gap-3">
              <TagOutlined className="-rotate-90 text-xl" />
              <span className="text-base font-medium">
              FIX Date
              </span>
              
            </div>
          ),
          children: <h2 className="text-base font-bold">30/10/2023</h2>,
        },
    ]
    return(
 <div className="w-full sm:w-3/4 overflow-hidden shadow-xl py-5 mx-auto">
        <h4 className="text-base bg-blue-200 h-10 flex items-center text-white pl-5 rounded-t-xl">Run End Of Day Fix</h4>
        <div>   
        <div className="w-4/5 mx-auto my-3 py-10">
        <Descriptions
        bordered
        column={{
          xs: 2,
          sm: 2,
          md: 2,
          lg: 2,
          xl: 2,
          xxl: 2,
        }}
        items={items}
        size='small'
        contentStyle={{backgroundColor:'#E8EDFF',width:'65%'}}
        labelStyle={{backgroundColor:'00#D8E3F8'}}
        
      />
      </div>
      <div className="md:w-2/4 mx-auto">
      <Progress percent={50} status="active" />
      </div>
      <div className="flex justify-center">
        <Form>
        <Form.Item name="reconcile with gl" valuePropName="checked">
              <Checkbox className="mr-5"><Button className="bg-blue-200 text-white border-none">Reconcile With GL</Button></Checkbox>
            </Form.Item>
            <Form.Item name="reconcile with trade" valuePropName="checked">
              <Checkbox><Button className="bg-blue-200 text-white border-none">Reconcile With Trade</Button></Checkbox>
            </Form.Item>
            <Form.Item name="reconcile with portfolio" valuePropName="checked">
              <Checkbox><Button className="bg-blue-200 text-white border-none">Reconcile With Portfolio</Button></Checkbox>
            </Form.Item>
            <Form.Item name="reconcile with trade file" valuePropName="checked">
              <Checkbox><Button>Reconcile With Trade File</Button></Checkbox>
            </Form.Item>
        </Form>
          
          </div>
        <div className="flex justify-end gap-5 px-10">
        <Button className="bg-red-250 text-white font-bold border-none">Cancel</Button>
        <Button className="bg-blue-200 text-white border-none">Run</Button>
        </div>
        </div>
     
       </div>
        
      
    )
}

export default EndOfTheDay